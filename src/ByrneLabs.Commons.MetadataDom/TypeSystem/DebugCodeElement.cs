using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public abstract class DebugCodeElement<TDebugCodeElement, THandle, TToken> : DebugCodeElement where TDebugCodeElement : DebugCodeElement<TDebugCodeElement, THandle, TToken>
    {
        internal DebugCodeElement(THandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<TDebugCodeElement>(metadataHandle), metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(MetadataHandle);
        }

        public int MetadataToken => MetadataHandle.GetHashCode();

        internal THandle MetadataHandle { get; }

        internal TToken RawMetadata { get; }
    }

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
