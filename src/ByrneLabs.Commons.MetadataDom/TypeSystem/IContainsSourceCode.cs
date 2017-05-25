namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public interface IContainsSourceCode
    {
        Document Document { get; }

        string SourceCode { get; }
    }
}
