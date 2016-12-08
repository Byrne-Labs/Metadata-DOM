using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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
