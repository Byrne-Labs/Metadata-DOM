namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal interface ICodeElementWithTypedHandle<out THandle, out TToken> : ICodeElementWithRawMetadata<TToken>
    {
        THandle MetadataHandle { get; }
    }
}
