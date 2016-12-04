using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class DebugCodeElementWithoutHandle : CodeElement, ICodeElementWithoutHandle
    {
        internal DebugCodeElementWithoutHandle(object metadata, MetadataState metadataState) : base(metadataState)
        {
            ((ICodeElementWithoutHandle) this).MetadataKey = metadata;
        }

        protected override sealed MetadataReader Reader => MetadataState.PdbReader;

        object ICodeElementWithoutHandle.MetadataKey { get; set; }
    }
}
