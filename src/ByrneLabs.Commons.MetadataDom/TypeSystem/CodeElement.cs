using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public abstract class CodeElement : ICodeElement
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

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey ICodeElement.Key => Key;

        MetadataState ICodeElement.MetadataState => MetadataState;

        internal SignatureDecoder<TypeBase, GenericContext> CreateSignatureDecoder(TypeDefinition genericContext) => new SignatureDecoder<TypeBase, GenericContext>(MetadataState.TypeProvider, Reader, new GenericContext(this, genericContext?.GenericTypeParameters, null));
    }
}
