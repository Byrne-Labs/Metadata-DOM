using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock" />
    //[PublicAPI]
    public class MethodBody : RuntimeCodeElement, ICodeElementWithRawMetadata<MethodBodyBlock>
    {
        private readonly Lazy<IEnumerable<ExceptionRegion>> _exceptionRegions;
        private readonly Lazy<StandaloneSignature> _localSignature;

        internal MethodBody(int relativeVirtualAddress, MetadataState metadataState) : base(new CodeElementKey<MethodBody>(relativeVirtualAddress), metadataState)
        {
            RawMetadata = MetadataState.GetMethodBodyBlock(relativeVirtualAddress);
            _exceptionRegions = MetadataState.GetLazyCodeElements<ExceptionRegion>(RawMetadata.ExceptionRegions);
            _localSignature = new Lazy<StandaloneSignature>(() =>
            {
                StandaloneSignature localSignature;
                if (RawMetadata.LocalSignature.IsNil)
                {
                    localSignature = null;
                }
                else
                {
                    localSignature = MetadataState.GetCodeElement<StandaloneSignature>(RawMetadata.LocalSignature);
                    localSignature.GenericContext = GenericContext;
                }
                return localSignature;
            });
            LocalVariablesInitialized = RawMetadata.LocalVariablesInitialized;
            MaxStack = RawMetadata.MaxStack;
            Size = RawMetadata.Size;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock.ExceptionRegions" />
        public IEnumerable<ExceptionRegion> ExceptionRegions => _exceptionRegions.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock.LocalSignature" />
        public StandaloneSignature LocalSignature => _localSignature.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock.LocalVariablesInitialized" />
        public bool LocalVariablesInitialized { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock.MaxStack" />
        public int MaxStack { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock.Size" />
        public int Size { get; }

        internal GenericContext GenericContext { get; set; }

        public MethodBodyBlock RawMetadata { get; }
    }
}
