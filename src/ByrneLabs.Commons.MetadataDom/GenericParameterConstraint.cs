using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint" />
    //[PublicAPI]
    public class GenericParameterConstraint : RuntimeCodeElement, ICodeElementWithHandle<GenericParameterConstraintHandle, System.Reflection.Metadata.GenericParameterConstraint>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<TypeBase> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<TypeBase>(() =>
            {
                var constrainedType = (TypeBase)MetadataState.GetCodeElement(MetadataToken.Type);
                var constrainedTypeSpecification = constrainedType as TypeSpecification;
                if (constrainedTypeSpecification != null)
                {
                    var parentType = Parameter.Parent as TypeDefinition;
                    if (parentType != null)
                    {
                        constrainedTypeSpecification.ParentTypeDefinition = parentType;
                    }
                }
                return constrainedType;
            });
            _parameter = MetadataState.GetLazyCodeElement<GenericParameter>(MetadataToken.Parameter);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Parameter" />
        /// <summary>The constrained <see cref="GenericParameter" />.</summary>
        public GenericParameter Parameter => _parameter.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Type" />
        /// <summary>Handle (<see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" />) specifying from which type this generic parameter is constrained to derive, or which interface this generic parameter is constrained to implement.</summary>
        public TypeBase Type => _type.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public GenericParameterConstraintHandle MetadataHandle { get; }

        public System.Reflection.Metadata.GenericParameterConstraint MetadataToken { get; }
    }
}
