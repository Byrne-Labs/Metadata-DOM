using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.Common
{
    public abstract class ReflectionComparison
    {
        protected abstract Assembly LoadAssembly(FileInfo assemblyFile);

        public IEnumerable<string> Check(FileInfo assemblyFile)
        {
            return Check(assemblyFile, null);
        }

        public int Check(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("No assembly file provided");
            }
            if (args.Length > 2)
            {
                throw new ArgumentException("Only an assembly file and an optionally pdb file can be specified");
            }

            var assemblyFile = new FileInfo(args[0]);
            FileInfo pdbFile = args.Length == 1 ? null : new FileInfo(args[1]);

            var errors = Check(assemblyFile, pdbFile);

            if (errors.Any())
            {
                File.WriteAllLines(assemblyFile.FullName + ".errors", errors);
            }
            return errors.Any() ? 1 : 0;
        }

    }
}