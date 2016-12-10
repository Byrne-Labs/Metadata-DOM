namespace ByrneLabs.Commons.MetadataDom
{
    public interface ICodeElementWithToken<out T>
    {
        T MetadataToken { get; }
    }
}
