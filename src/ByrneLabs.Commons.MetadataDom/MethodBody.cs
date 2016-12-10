﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodBodyBlock" />
    [PublicAPI]
    public class MethodBody : RuntimeCodeElement, ICodeElementWithToken<MethodBodyBlock>
    {
        private readonly Lazy<IEnumerable<ExceptionRegion>> _exceptionRegions;
        private readonly Lazy<StandaloneSignature> _localSignature;

        internal MethodBody(int relativeVirtualAddress, MetadataState metadataState) : base(new HandlelessCodeElementKey<MethodBody>(relativeVirtualAddress), metadataState)
        {
            MetadataToken = MetadataState.GetMethodBodyBlock(relativeVirtualAddress);
            _exceptionRegions = GetLazyCodeElementsWithoutHandle<ExceptionRegion>(MetadataToken.ExceptionRegions);
            _localSignature = GetLazyCodeElementWithHandle<StandaloneSignature>(MetadataToken.LocalSignature);
            LocalVariablesInitialized = MetadataToken.LocalVariablesInitialized;
            MaxStack = MetadataToken.MaxStack;
            Size = MetadataToken.Size;
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

        public MethodBodyBlock MetadataToken { get; }
    }
}
