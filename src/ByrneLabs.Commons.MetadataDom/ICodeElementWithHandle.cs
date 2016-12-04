using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    internal interface ICodeElementWithHandle
    {
        Handle? DowncastMetadataHandle { get; set; }

        object MetadataHandle { get; set; }
    }
}
