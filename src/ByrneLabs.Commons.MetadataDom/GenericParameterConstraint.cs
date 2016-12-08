using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint" />
    [PublicAPI]
    public class GenericParameterConstraint : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<CodeElement> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var genericParameterConstraint = Reader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<CodeElement>(() => GetCodeElement(genericParameterConstraint.Type));
            _parameter = new Lazy<GenericParameter>(() => GetCodeElement<GenericParameter>(genericParameterConstraint.Parameter));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(genericParameterConstraint.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Parameter" />
        /// <summary>The constrained <see cref="T:ByrneLabs.Commons.MetadataDom.GenericParameter" />.</summary>
        public GenericParameter Parameter => _parameter.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameterConstraint.Type" />
        /// <summary>Handle (<see cref="T:ByrneLabs.Commons.MetadataDom.TypeDefinition" />, <see cref="T:ByrneLabs.Commons.MetadataDom.TypeReference" />, or
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeSpecification" />) specifying from which type this generic parameter is constrained to derive, or which interface this generic parameter is constrained to implement.</summary>
        public CodeElement Type => _type.Value;
    }
}
