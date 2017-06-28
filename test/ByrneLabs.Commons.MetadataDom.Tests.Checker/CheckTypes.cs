using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    [Flags]
    public enum CheckTypes
    {
        MetadataTypes = 1,
        MetadataSymbols = 2,
        Metadata = 3,
        ReflectionComparison = 4,
        Everything = 7
    }
}
