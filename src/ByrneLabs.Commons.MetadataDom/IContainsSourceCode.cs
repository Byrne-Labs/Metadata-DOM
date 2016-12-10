using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public interface IContainsSourceCode
    {
        Document Document { get; }

        string SourceCode { get; }
    }
}
