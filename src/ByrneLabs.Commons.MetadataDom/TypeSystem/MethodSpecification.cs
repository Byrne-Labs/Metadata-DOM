using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class MethodSpecification : MemberReferenceBase
    {
        private readonly Lazy<ImmutableArray<CustomAttributeData>> _customAttributes;
        private readonly Lazy<MethodBase> _method;
        private readonly Lazy<ImmutableArray<TypeBase>> _signature;

        internal MethodSpecification(MethodSpecificationHandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<MethodSpecification>(metadataHandle), metadataState)
        {
            RawMetadata = MetadataState.AssemblyReader.GetMethodSpecification(metadataHandle);
            
            _method = new Lazy<MethodBase>(() => (MethodBase)MetadataState.GetCodeElement(new CodeElementKey(RawMetadata.Method.GetType(), Method, Signature)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute, CustomAttributeData>(RawMetadata.GetCustomAttributes());
            _signature = new Lazy<ImmutableArray<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, null));
        }

        public System.Reflection.Metadata.MethodSpecification RawMetadata { get; }

        public ImmutableArray<CustomAttributeData> CustomAttributes => _customAttributes.Value;

        public TypeBase DeclaringType => Method.DeclaringType;

        public string FullName => Method.FullName;

        public ImmutableArray<GenericParameter> GenericTypeParameters => Method.GenericTypeParameters;

        public bool IsGenericMethod => Method.IsGenericMethod;
        public MethodBase Method => _method.Value;

        public string Name => Method.Name;

        public ImmutableArray<Parameter> Parameters => Method.Parameters;

        public string TextSignature => Method.TextSignature;

        internal ImmutableArray<TypeBase> Signature => _signature.Value;
    }
}
