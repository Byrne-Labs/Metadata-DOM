using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class CodeElementWithoutHandle : CodeElement, ICodeElementWithoutHandle
    {
        internal CodeElementWithoutHandle(object metadataKey, MetadataState metadataState) : base(metadataState)
        {
            ((ICodeElementWithoutHandle) this).MetadataKey = metadataKey;
            metadataState.CacheCodeElement(this);
        }

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader;

        object ICodeElementWithoutHandle.MetadataKey { get; set; }
    }
}
