using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class MethodBody : System.Reflection.MethodBody, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<ExceptionRegion>> _exceptionRegions;
        private readonly Lazy<StandaloneSignature> _localSignature;

        internal MethodBody(int relativeVirtualAddress, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodBody>(relativeVirtualAddress);
            MetadataState = metadataState;
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
                    localSignature = MetadataState.GetCodeElement<StandaloneSignature>(RawMetadata.LocalSignature, GenericContext);
                }
                return localSignature;
            });
            InitLocals = RawMetadata.LocalVariablesInitialized;
            MaxStackSize = RawMetadata.MaxStack;
            Size = RawMetadata.Size;
        }

        public override IList<ExceptionHandlingClause> ExceptionHandlingClauses => _exceptionRegions.Value.ToImmutableList<ExceptionHandlingClause>();

        public string FullName => LocalSignature.FullName;

        public override bool InitLocals { get; }

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public override int MaxStackSize { get; }

        public string Name => LocalSignature.Name;

        public MethodBodyBlock RawMetadata { get; }

        public int Size { get; }

        public string TextSignature => LocalSignature.TextSignature;

        internal GenericContext GenericContext { get; set; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
