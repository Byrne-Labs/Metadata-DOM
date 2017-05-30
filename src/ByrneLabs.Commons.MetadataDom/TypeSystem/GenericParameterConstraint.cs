using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class GenericParameterConstraint : IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<TypeBase> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            Key = new CodeElementKey<GenericParameterConstraint>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<TypeBase>(() =>
            {
                TypeBase constrainedType;
                if (RawMetadata.Type.Kind == HandleKind.TypeSpecification)
                {
                    constrainedType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Type, Parameter.Parent);
                }
                else
                {
                    constrainedType = (TypeBase) MetadataState.GetCodeElement(RawMetadata.Type);
                }
                return constrainedType;
            });
            _parameter = MetadataState.GetLazyCodeElement<GenericParameter>(RawMetadata.Parameter);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public GenericParameterConstraintHandle MetadataHandle { get; }

        public GenericParameter Parameter => _parameter.Value;

        public System.Reflection.Metadata.GenericParameterConstraint RawMetadata { get; }

        public TypeBase Type => _type.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
