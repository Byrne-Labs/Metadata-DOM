using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint" />
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
                var constrainedType = (TypeBase) MetadataState.GetCodeElement(RawMetadata.Type);
                var constrainedTypeSpecification = constrainedType as TypeSpecification;
                if (constrainedTypeSpecification != null)
                {
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull -- Using try cast for all possible classes would not be faster than checking the type. -- Jonathan Byrne 01/21/2017
                    if (Parameter.Parent is TypeDefinition)
                    {
                        constrainedTypeSpecification.ParentTypeDefinition = (TypeDefinition) Parameter.Parent;
                    }
                    else if (Parameter.Parent is MethodDefinition)
                    {
                        constrainedTypeSpecification.ParentMethodDefinition = (MethodDefinition) Parameter.Parent;
                    }
                }
                return constrainedType;
            });
            _parameter = MetadataState.GetLazyCodeElement<GenericParameter>(RawMetadata.Parameter);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Parameter" />
        /// <summary>The constrained <see cref="GenericParameter" />.</summary>
        public GenericParameter Parameter => _parameter.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Type" />
        /// <summary>Handle (<see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" />) specifying from which type this generic parameter is constrained to derive, or which interface this generic parameter is constrained to implement.</summary>
        public TypeBase Type => _type.Value;

        public System.Reflection.Metadata.GenericParameterConstraint RawMetadata { get; }

        public GenericParameterConstraintHandle MetadataHandle { get; }
    }
}
