namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    internal static class ExtensionsSamples
    {
        private static string Something(this string original, string somethingElse)
        {
            return somethingElse + original;
        }
    }
}
