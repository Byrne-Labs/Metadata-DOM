using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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
