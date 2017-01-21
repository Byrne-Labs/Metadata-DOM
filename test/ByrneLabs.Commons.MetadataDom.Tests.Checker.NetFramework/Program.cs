using System;
using System.Diagnostics.CodeAnalysis;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Entry point for application")]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var checker = new NetFrameworkChecker(args);
            var exitCode = checker.Check().Success ? 0 : 1;
            Environment.Exit(exitCode);
        }
    }
}
