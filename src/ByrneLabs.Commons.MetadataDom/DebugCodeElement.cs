using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class DebugCodeElement : CodeElement
    {
        internal DebugCodeElement(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal DebugCodeElement(Handle metadataHandle, MetadataState metadataState) : this(new CodeElementKey(metadataHandle), metadataState)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override sealed MetadataReader Reader => MetadataState.PdbReader;
    }
}
