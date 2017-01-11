using System;
using System.IO;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("An assembly file name must be provided");
            }
            if (args.Length > 2)
            {
                throw new ArgumentException("Only an assembly file name and PDB file name can be provided");
            }

            bool success;
            if (args.Length == 1)
            {
                success = ReflectionChecker.Check(new FileInfo(args[0]));
            }
            else
            {
                success = ReflectionChecker.Check(new FileInfo(args[0]), new FileInfo(args[0]));
            }

            Environment.Exit(success ? 0 : 1);
        }
    }
}
