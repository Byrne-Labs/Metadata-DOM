using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Reflection;
#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public partial class MethodBody : IManagedCodeElement
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
                    localSignature = MetadataState.GetCodeElement<StandaloneSignature>(RawMetadata.LocalSignature);
                    localSignature.GenericContext = GenericContext;
                }
                return localSignature;
            });
            InitLocals = RawMetadata.LocalVariablesInitialized;
            MaxStackSize = RawMetadata.MaxStack;
            Size = RawMetadata.Size;
        }

        public override IList<ExceptionHandlingClause> ExceptionHandlingClauses => _exceptionRegions.Value.ToImmutableList<ExceptionHandlingClause>();

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public override bool InitLocals { get; }

        public override int MaxStackSize { get; }

        public MethodBodyBlock RawMetadata { get; }

        public int Size { get; }

        internal GenericContext GenericContext { get; set; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        public string FullName => LocalSignature.FullName;

        public string Name => LocalSignature.Name;

        public string TextSignature => LocalSignature.TextSignature;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class MethodBody : System.Reflection.MethodBody
    {
    }
#else
    public partial class MethodBody : ByrneLabs.Commons.MetadataDom.MethodBody
    {
        public override int LocalSignatureMetadataToken { get; }

        public override IList<LocalVariableInfo> LocalVariables { get; }

        public override byte[] GetILAsByteArray() => throw new NotImplementedException();
    }
#endif

}
