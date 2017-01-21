namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
   static class ExtensionsSamples
    {
        static string Something(this string original, string somethingElse)
        {
            return somethingElse + original;
        }
    }
}
