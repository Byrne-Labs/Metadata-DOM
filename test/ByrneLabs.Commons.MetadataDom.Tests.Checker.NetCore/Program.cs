using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Entry point for program")]
    internal class Program
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Entry point for program must be string array")]
        private static void Main(string[] args)
        {
            var checker = new NetCoreChecker(args);
            var exitCode = checker.Check().Success ? 0 : 1;
            Environment.Exit(exitCode);
        }
    }
}
