using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class CodeElementWithHandle : CodeElement, ICodeElementWithHandle
    {
        internal CodeElementWithHandle(object metadataHandle, MetadataState metadataState) : base(metadataState)
        {
            ((ICodeElementWithHandle) this).MetadataHandle = metadataHandle;
            ((ICodeElementWithHandle) this).DowncastMetadataHandle = MetadataState.DowncastHandle(metadataHandle);
            metadataState.CacheCodeElement(this);
        }

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader;

        Handle? ICodeElementWithHandle.DowncastMetadataHandle { get; set; }

        object ICodeElementWithHandle.MetadataHandle { get; set; }
    }
}
