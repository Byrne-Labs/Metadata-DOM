using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.Common
{
    public abstract class ReflectionComparison
    {
        protected abstract Assembly LoadAssembly(FileInfo assemblyFile);

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

            using (var reflectionData = (pdbFile == null ? new ReflectionData(assemblyFile) : new ReflectionData(assemblyFile, pdbFile)))
            {
                var assembly = LoadAssembly(assemblyFile);
                var errors = ReflectionChecker.CompareCodeElementsToReflectionData(assembly, reflectionData);
                if (errors.Any())
                {
                    File.WriteAllLines(assemblyFile.FullName + ".errors", errors);
                }
                return errors.Any() ? 1 : 0;
            }
        }

    }
}