using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class GenericParameter : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<GenericParameterConstraint>> _constraints;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;
        private readonly Lazy<CodeElement> _parent;

        internal GenericParameter(GenericParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var genericParameter = Reader.GetGenericParameter(metadataHandle);
            _name = new Lazy<string>(() => AsString(genericParameter.Name));
            Attributes = genericParameter.Attributes;
            Index = genericParameter.Index;
            _parent = new Lazy<CodeElement>(() => GetCodeElement(genericParameter.Parent));
            _constraints = new Lazy<IEnumerable<GenericParameterConstraint>>(() => GetCodeElements<GenericParameterConstraint>(genericParameter.GetConstraints()));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(genericParameter.GetCustomAttributes()));
        }

        public GenericParameterAttributes Attributes { get; }

        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public int Index { get; }

        public string Name => _name.Value;

        public CodeElement Parent => _parent.Value;
    }
}
