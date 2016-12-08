using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public interface IContainsSourceCode
    {
        string SourceCode { get; }

        string SourceFile { get; }
    }
}
