using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class StandaloneSignature : IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<TypeBase>> _localSignature;
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
                _localSignature = new Lazy<IEnumerable<TypeBase>>(() => RawMetadata.DecodeLocalSignature(MetadataState.TypeProvider, GenericContext));
            }
            else
            {
                _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, GenericContext));
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public string FullName => throw NotSupportedHelper.FutureVersion();

        public StandaloneSignatureKind Kind { get; }

        public IEnumerable<TypeBase> LocalVariableSignature => Kind == StandaloneSignatureKind.LocalVariables ? _localSignature.Value : throw new InvalidOperationException("This property is only valid when the signature kind is local variable");

        public StandaloneSignatureHandle MetadataHandle { get; }

        public MethodSignature<TypeBase> MethodSignature => Kind == StandaloneSignatureKind.Method ? _methodSignature.Value : throw new InvalidOperationException("This property is only valid when the signature kind is method");

        public string Name => throw NotSupportedHelper.FutureVersion();

        public System.Reflection.Metadata.StandaloneSignature RawMetadata { get; }

        public string TextSignature => throw NotSupportedHelper.FutureVersion();

        internal GenericContext GenericContext { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
