using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.NetCore
{
    public class ReflectionComparisonNetCore : Common.ReflectionComparison
    {
        public static int Main(string[] args)
        {
            return new ReflectionComparisonNetCore().Check(args);
        }

        protected override Assembly LoadAssembly(FileInfo assemblyFile)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile.FullName);
        }
    }
}
