using System.IO;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.NetFramework
{
    public class ReflectionComparisonNetFramework : Common.ReflectionComparison
    {
        public static int Main(string[] args)
        {
            return new ReflectionComparisonNetFramework().Check(args);
        }

        protected override Assembly LoadAssembly(FileInfo assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile.FullName);
        }
    }
}