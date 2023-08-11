using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public class DatabaseAssemblyResolver : IAssemblyResolver
    {
        List<SqlAssemblyObject> _databaseLibs;
        DefaultAssemblyResolver _resolver;
        public DatabaseAssemblyResolver(List<SqlAssemblyObject> dlls)
        {
            _databaseLibs = dlls;
            _resolver = new DefaultAssemblyResolver();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            foreach(var item in _databaseLibs)
            {
                if (item.AssemblyName.Contains(name.Name))
                {
                    return AssemblyDefinition.ReadAssembly(new MemoryStream(item.Data.Value));
                }
            }
            return _resolver.Resolve(name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            foreach (var item in _databaseLibs)
            {
                if (item.AssemblyName.Contains(name.Name))
                {
                    return AssemblyDefinition.ReadAssembly(new MemoryStream(item.Data.Value));
                }
            }
            return _resolver.Resolve(name, parameters);
        }
    }
}
