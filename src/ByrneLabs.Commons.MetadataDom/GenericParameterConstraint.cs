﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public GenericParameter Parameter => _parameter.Value;

        public CodeElement Type => _type.Value;
    }
}
