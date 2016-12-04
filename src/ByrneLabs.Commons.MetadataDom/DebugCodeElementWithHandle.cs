using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class DebugCodeElementWithHandle : CodeElement, ICodeElementWithHandle
    {
        internal DebugCodeElementWithHandle(object metadataHandle, MetadataState metadataState) : base(metadataState)
        {
            ((ICodeElementWithHandle) this).MetadataHandle = metadataHandle;
            ((ICodeElementWithHandle) this).DowncastMetadataHandle = MetadataState.DowncastHandle(metadataHandle);
        }

        protected override sealed MetadataReader Reader => MetadataState.PdbReader;

        Handle? ICodeElementWithHandle.DowncastMetadataHandle { get; set; }

        object ICodeElementWithHandle.MetadataHandle { get; set; }
    }
}
