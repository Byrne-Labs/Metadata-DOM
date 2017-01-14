using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var checker = new NetFrameworkChecker(args);
            var exitCode = checker.Check().Success ? 0 : 1;
            Environment.Exit(exitCode);
        }
    }
}
