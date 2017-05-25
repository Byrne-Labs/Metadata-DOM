using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class StandaloneSignature : IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<TypeBase>> _localSignature;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;

        internal StandaloneSignature(StandaloneSignatureHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetStandaloneSignature(metadataHandle);
            _localSignature = new Lazy<ImmutableArray<TypeBase>>(() => RawMetadata.DecodeLocalSignature(MetadataState.TypeProvider, GenericContext));
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, GenericContext));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Kind = RawMetadata.GetKind();
        }

        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public StandaloneSignatureKind Kind { get; }

        public ImmutableArray<TypeBase> LocalVariableSignagure => _localSignature.Value;

        public StandaloneSignatureHandle MetadataHandle { get; }

        public MethodSignature<TypeBase> MethodSignature => _methodSignature.Value;

        public System.Reflection.Metadata.StandaloneSignature RawMetadata { get; }

        internal GenericContext GenericContext { get; set; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public string FullName => throw new NotSupportedException();

        public string Name => throw new NotSupportedException();

        public string TextSignature => throw new NotSupportedException();
    }
}
