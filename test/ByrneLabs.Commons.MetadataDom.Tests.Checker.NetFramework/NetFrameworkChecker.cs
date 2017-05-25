﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework
{
    internal class NetFrameworkChecker : BaseChecker
    {
        public NetFrameworkChecker(IReadOnlyList<string> args) : base(args)
        {
        }

        public NetFrameworkChecker(DirectoryInfo baseDirectory, FileInfo assemblyFile, FileInfo pdbFile = null) : base(baseDirectory, assemblyFile, pdbFile)
        {
        }

        protected override System.Reflection.Assembly LoadAssembly() => Assembly.LoadFile(AssemblyFile.FullName);
    }
}
