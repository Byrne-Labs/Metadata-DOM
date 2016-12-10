using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    internal interface ICodeElementWithHandle<out THandle, out TToken> : ICodeElementWithToken<TToken>
    {
        Handle DowncastMetadataHandle { get; }

        THandle MetadataHandle { get; }
    }
}
