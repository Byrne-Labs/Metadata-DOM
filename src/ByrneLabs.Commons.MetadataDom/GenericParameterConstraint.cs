using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class GenericParameterConstraint : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<CodeElement> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var genericParameterConstraint = Reader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<CodeElement>(() => GetCodeElement(genericParameterConstraint.Type));
            _parameter = new Lazy<GenericParameter>(() => GetCodeElement<GenericParameter>(genericParameterConstraint.Parameter));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(genericParameterConstraint.GetCustomAttributes()));
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public GenericParameter Parameter => _parameter.Value;

        public CodeElement Type => _type.Value;
    }
}
