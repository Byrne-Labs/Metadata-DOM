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

        internal StandaloneSignature(StandaloneSignatureHandle metadataHandle, GenericContext genericContext, MetadataState metadataState)
        {
            Key = new CodeElementKey<StandaloneSignature>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetStandaloneSignature(metadataHandle);
            GenericContext = genericContext;
            Kind = RawMetadata.GetKind();
            if (Kind == StandaloneSignatureKind.LocalVariables)
            {
                _localSignature = new Lazy<ImmutableArray<TypeBase>>(() => RawMetadata.DecodeLocalSignature(MetadataState.TypeProvider, GenericContext));
            }
            else
            {
                _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, GenericContext));
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public string FullName => throw new NotSupportedException();

        public StandaloneSignatureKind Kind { get; }

        public ImmutableArray<TypeBase> LocalVariableSignature => Kind == StandaloneSignatureKind.LocalVariables ? _localSignature.Value : throw new InvalidOperationException("This property is only valid when the signature kind is local variable");

        public StandaloneSignatureHandle MetadataHandle { get; }

        public MethodSignature<TypeBase> MethodSignature => Kind == StandaloneSignatureKind.Method ? _methodSignature.Value : throw new InvalidOperationException("This property is only valid when the signature kind is method");

        public string Name => throw new NotSupportedException();

        public System.Reflection.Metadata.StandaloneSignature RawMetadata { get; }

        public string TextSignature => throw new NotSupportedException();

        internal GenericContext GenericContext { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
