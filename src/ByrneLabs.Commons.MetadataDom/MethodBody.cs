using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock" />
    [PublicAPI]
    public class MethodBody : CodeElementWithoutHandle
    {
        private readonly Lazy<IReadOnlyList<ExceptionRegion>> _exceptionRegions;
        private readonly Lazy<StandaloneSignature> _localSignature;

        internal MethodBody(int relativeVirtualAddress, MetadataState metadataState) : base(new HandlelessCodeElementKey<MethodBody>(relativeVirtualAddress), metadataState)
        {
            var methodBody = MetadataState.GetMethodBodyBlock(relativeVirtualAddress);
            _exceptionRegions = GetLazyCodeElements<ExceptionRegion>(methodBody.ExceptionRegions.Select(exceptionRegion => new HandlelessCodeElementKey<ExceptionRegion>(exceptionRegion)));
            _localSignature = GetLazyCodeElement<StandaloneSignature>(methodBody.LocalSignature);
            LocalVariablesInitialized = methodBody.LocalVariablesInitialized;
            MaxStack = methodBody.MaxStack;
            Size = methodBody.Size;
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
    }
}
