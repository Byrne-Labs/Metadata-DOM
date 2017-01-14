using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var checker = new NetCoreChecker(args);
            var exitCode = checker.Check().Success ? 0 : 1;
            Environment.Exit(exitCode);
        }
    }
}