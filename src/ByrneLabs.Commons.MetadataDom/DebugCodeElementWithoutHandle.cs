using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class DebugCodeElementWithoutHandle : CodeElement, ICodeElementWithoutHandle
    {
        internal DebugCodeElementWithoutHandle(object metadata, MetadataState metadataState) : base(metadataState)
        {
            ((ICodeElementWithoutHandle)this).MetadataKey = metadata;
        }

        protected override sealed MetadataReader Reader => MetadataState.PdbReader;

        object ICodeElementWithoutHandle.MetadataKey { get; set; }
    }
}
