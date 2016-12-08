using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter" />
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Attributes" />
        public GenericParameterAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetConstraints" />
        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Index" />
        public int Index { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Parent" />
        /// <summary>
        ///     <see cref="ByrneLabs.Commons.MetadataDom.TypeDefinition" /> or <see cref="ByrneLabs.Commons.MetadataDom.MethodDefinition" />.</summary>
        public CodeElement Parent => _parent.Value;
    }
}
