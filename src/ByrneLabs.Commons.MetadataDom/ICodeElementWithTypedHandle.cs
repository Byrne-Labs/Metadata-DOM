namespace ByrneLabs.Commons.MetadataDom
{
    internal interface ICodeElementWithTypedHandle<out THandle, out TToken> : ICodeElementWithRawMetadata<TToken>
    {
        THandle MetadataHandle { get; }
    }
}
