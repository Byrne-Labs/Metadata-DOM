namespace ByrneLabs.Commons.MetadataDom
{
    public interface ICodeElementWithRawMetadata<out T>
    {
        T RawMetadata { get; }
    }
}
