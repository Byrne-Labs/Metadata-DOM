using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore
{
    internal class NetCoreChecker : BaseChecker
    {
        public NetCoreChecker(IReadOnlyList<string> args) : base(args)
        {
        }

        public NetCoreChecker(DirectoryInfo baseDirectory, FileInfo assemblyFile, FileInfo pdbFile = null) : base(baseDirectory, assemblyFile, pdbFile)
        {
        }

        public static CheckState Check(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checker = new NetCoreChecker(null, assemblyFile, pdbFile);
            return checker.Check(CheckTypes.Everything);
        }

        protected override System.Reflection.Assembly LoadAssembly() => AssemblyLoadContext.Default.LoadFromAssemblyPath(AssemblyFile.FullName);
    }
}
