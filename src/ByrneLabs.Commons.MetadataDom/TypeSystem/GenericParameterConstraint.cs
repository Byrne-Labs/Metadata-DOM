using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class GenericParameterConstraint : RuntimeCodeElement, ICodeElementWithTypedHandle<GenericParameterConstraintHandle, System.Reflection.Metadata.GenericParameterConstraint>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<TypeBase> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<TypeBase>(() =>
            {
                TypeBase constrainedType;
                if (RawMetadata.Type.Kind == HandleKind.TypeSpecification)
                {
                    constrainedType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Type, Parameter.Parent);
                }
                else
                {
                    constrainedType = (TypeBase)MetadataState.GetCodeElement(RawMetadata.Type);
                }
                return constrainedType;
            });
            _parameter = MetadataState.GetLazyCodeElement<GenericParameter>(RawMetadata.Parameter);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;
        public GenericParameter Parameter => _parameter.Value;
        public TypeBase Type => _type.Value;

        public System.Reflection.Metadata.GenericParameterConstraint RawMetadata { get; }

        public GenericParameterConstraintHandle MetadataHandle { get; }
    }
}
