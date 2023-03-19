using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Utils
{
    public class StringUtils
    {
        
        public static int GetHash(IEnumerable<string> strs, int tableSize)
        {
            int n = strs.Count();
            int h = 0, c = 0;
            string str_i = null;

            for(int i = 0; i < n; i++)
            {
                str_i = strs.ElementAt(i);
                for(int j = 0; j < str_i.Length; j++){
                    c = str_i[j];

                    checked
                    {   //enable overflow checking
                        try
                        {
                            h = 31 * h + str_i[j];
                        } catch (OverflowException e)
                        {
                            h = 0;
                            h = 31 * h + str_i[j];
                        }
                    }
                }
            }

            return h % tableSize;
        }

        public static bool Mathes(string[] a, string[] b)
        {
            if (a == b || (a == null && b == null))
                return true;
            if (a == null || b == null)
                return false;
            if(a.Length != b.Length) return false;
            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i] || (a[i] == null && b[i] == null)) continue;
                if (a[i] == null || b[i] == null) return false;
                if (!a[i].Equals(b[i])) return false;
            }

            return true;
        }
    }
}
