using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore
{
    [PublicAPI]
    public class Program
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Entry point for program must be string array")]
        private static void Main(string[] args)
        {
            var checker = new NetCoreChecker(args);
            var exitCode = checker.Check(CheckTypes.Everything).Success ? 0 : 1;
            Environment.Exit(exitCode);
        }
    }
}
