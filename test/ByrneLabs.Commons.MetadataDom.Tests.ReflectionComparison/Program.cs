using System;
using System.IO;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("A base directory and assembly file name must be provided");
            }
            if (args.Length > 3)
            {
                throw new ArgumentException("Only a base directory, assembly file name, and PDB file name can be provided");
            }

            ReflectionChecker.BaseDirectory = args[0];
            bool success;
            if (args.Length == 2)
            {
                success = ReflectionChecker.Check(new FileInfo(args[1]));
            }
            else
            {
                success = ReflectionChecker.Check(new FileInfo(args[1]), new FileInfo(args[2]));
            }

            Environment.Exit(success ? 0 : 1);
        }
    }
}
