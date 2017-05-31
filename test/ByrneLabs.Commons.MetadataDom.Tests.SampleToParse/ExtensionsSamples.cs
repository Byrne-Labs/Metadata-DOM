namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    internal static class ExtensionsSamples
    {
#if CSHARP_V6 && DOTNET_V3_5
        private static string Something(this string original, string somethingElse) => somethingElse + original;
#endif
    }
}
