using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework
{
    public class NetFrameworkChecker : BaseChecker
    {

        static NetFrameworkChecker()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (sender, args) => Assembly.ReflectionOnlyLoad(args.Name);
        }

        public NetFrameworkChecker(IReadOnlyList<string> args) : base(args)
        {
        }

        public NetFrameworkChecker(DirectoryInfo baseDirectory, FileInfo assemblyFile, FileInfo pdbFile = null) : base(baseDirectory, assemblyFile, pdbFile)
        {
        }

        public static CheckState Check(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checker = new NetFrameworkChecker(null, assemblyFile, pdbFile);
            return checker.Check();
        }

        protected override Assembly LoadAssembly() => Assembly.ReflectionOnlyLoadFrom(AssemblyFile.FullName);
    }
}
