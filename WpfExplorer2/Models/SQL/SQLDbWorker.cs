using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using ICSharpCode.Decompiler;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WpfExplorer.Models
{
    public class SQLDbWorker : IDisposable
    {
        private SqlConnection _conn;
        private bool _connected;
        private bool _disposed;

        public SQLDbWorker(string conn)
        {
            _conn = new SqlConnection(conn);
            _conn.Open();
            _connected = true;
        }

        public bool IsConnected { get { return _connected; } }

        public void SetConnection(string connectionString)
        {
            if(_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");
            if(_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
            _conn = new SqlConnection(connectionString);
            _conn.Open();

        }


        public async Task<bool> AlterProcedureAsync(string stmt)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");
            if (string.IsNullOrWhiteSpace(stmt))
                throw new ArgumentException("Cannot alter: empty body!");
            bool r = true;
            using (SqlCommand c = _conn.CreateCommand())
            {
                c.CommandText = stmt;
                c.CommandType = CommandType.Text;
                try
                {
                    await c.ExecuteNonQueryAsync();
                } catch(Exception ex)
                {
                    r = false;
                }
            }
            return r;
        }

        //FIND string stmt in procedures.
        public async Task<Dictionary<string, string>> FindString(string stmt)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");
            if (string.IsNullOrWhiteSpace(stmt))
                throw new ArgumentException("Search string cannot be empty or null!");

            
            Dictionary<string, string> results = new Dictionary<string, string>();
            Regex regex = new Regex("[\\s]{2,}", RegexOptions.None);

            string definition = "";
            string procName = "";
            using (SqlCommand c = _conn.CreateCommand())
            {
                c.CommandType = CommandType.Text;
                c.CommandText = "select a.name, b.definition from sys.objects (nolock) a " + 
                                "join sys.sql_modules b (nolock) on b.[object_id] = a.[object_id]" + 
                                "where a.[type] IN ('FN', 'AF', 'IF', 'TF', 'P', 'V')" + 
                                "and b.definition like '%" + stmt + "%' ";
                SqlDataReader reader = null;
                try
                {
                    reader = await c.ExecuteReaderAsync(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        procName =   reader.GetValue(0) as string;
                        definition = reader.GetValue(1) as string;
                        definition = regex.Replace(definition, "\n");
                        definition = definition.Replace("CREATE", "ALTER");
                        results.Add(procName, definition);
                    }
                }
                catch (Exception ex)
                {
                    var emsg = ex.Message;
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                }
            }
            return results;
        }



        //Show object definition
        public async Task<string> GetObjectDefinitionAsync(string objectName)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");

            string definition = await GetProcedureDefinitionAsync(objectName, "dbo.sp_helptextEx");
            if (string.IsNullOrEmpty(definition))
                definition = await GetProcedureDefinitionAsync(objectName, "dbo.sp_helptext");

            return definition;
        }

        public async Task<List<SqlAssemblyObject>> GetAssemblyDefinitionAsync(string objectName)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");

            List<SqlAssemblyObject> results = new List<SqlAssemblyObject>();

            using (SqlCommand c = _conn.CreateCommand())
            {
                c.CommandType = CommandType.Text;
                c.CommandText = "select af.name, af.content from sys.assemblies a with (nolock) " +
                                "inner join sys.assembly_files af with (nolock) on a.assembly_id = af.assembly_id " +
                                "where af.name like '%" + objectName + "%' ";


                SqlDataReader reader = null;
                try
                {
                    reader = await c.ExecuteReaderAsync(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        results.Add(new SqlAssemblyObject() { AssemblyName = reader.GetValue(0) as string, Data = reader.GetSqlBytes(1) });
                    }
                }
                catch(Exception ex) {
                    var emsg = ex.Message;
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                }
            }
            return results;
        }


        //Show definition of procedures and also views.
        private async Task<string> GetProcedureDefinitionAsync(string procName, string metaProcName)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");
            
            string definition = "";
            using (SqlCommand c = _conn.CreateCommand())
            {
                c.CommandType = CommandType.StoredProcedure;
                c.CommandText = metaProcName;
                c.Parameters.AddWithValue(metaProcName == "dbo.sp_helptextEx" ? "Object" : "objname", procName); //object for Ex, objName fro 
                SqlDataReader reader = null;

                try
                {
                    reader = await c.ExecuteReaderAsync(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        definition += reader.GetValue(0) as string;
                    }
                }
                catch (Exception ex)
                {
                    var emsg = ex.Message;
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                }
            }
            return definition.Replace("CREATE", "ALTER");
        }

        public async Task<DataSet> QueryAsync(string sql)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");

            DataSet ds = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(sql, _conn);
            try
            {
                reader = await command.ExecuteReaderAsync(CommandBehavior.KeyInfo);
                ds = await Task.Run(() => convertDataReaderToDataSet(reader));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                if (!reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return ds;
        }
        public DataSet Query(string sql)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName + " was Disposed!");
            DataSet ds = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(sql, _conn);
            try
            {
                reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                ds = convertDataReaderToDataSet(reader);

            }catch(Exception ex)
            {

            }
            finally
            {

                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return ds;
        }

        private DataSet convertDataReaderToDataSet(SqlDataReader reader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                DataTable schemaTable = reader.GetSchemaTable();
                DataTable dataTable = new DataTable();

                int k = 1;
                if (schemaTable != null)
                {
                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        DataRow dataRow = schemaTable.Rows[i];
                        string columnName = (string)dataRow["ColumnName"];

                        string name = columnName;
                        while (dataTable.Columns[name] != null)
                        {
                            name = columnName + k;
                            k++;
                        }
                        DataColumn column = new DataColumn(name, (Type)dataRow["DataType"]);

                        column.Caption = columnName;
                        dataTable.Columns.Add(column);
                    }

                    dataSet.Tables.Add(dataTable);

                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataRow[i] = reader.GetValue(i);
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    //no records do nothing...
                }
            }
            while (reader.NextResult());
            return dataSet;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing) //Called Not by GC.
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
                _connected = false;
                
            }

            //GC Called.

            _disposed = true;
        }

    }
}
