namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal interface ICodeElementWithRawMetadata<out T> : ICodeElement
    {
        T RawMetadata { get; }
    }
}
