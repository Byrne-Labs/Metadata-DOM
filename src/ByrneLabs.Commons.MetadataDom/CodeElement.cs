using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class CodeElement
    {
        internal CodeElement(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            MetadataState = metadataState;
            metadataState.CacheCodeElement(this, key);
        }

        internal CodeElement(Handle metadataHandle, MetadataState metadataState) : this(new CodeElementKey(metadataHandle), metadataState)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract MetadataReader Reader { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        internal CodeElementKey Key { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal MetadataState MetadataState { get; }

        protected Guid AsGuid(GuidHandle guidHandle) => Reader.GetGuid(guidHandle);

        protected string AsString(StringHandle stringHandle) => Reader.GetString(stringHandle);

        internal SignatureDecoder<TypeBase, GenericContext> CreateSignatureDecoder(TypeDefinition genericContext) => new SignatureDecoder<TypeBase, GenericContext>(MetadataState.TypeProvider, Reader, new GenericContext(genericContext?.GenericTypeParameters, null));
    }
}
