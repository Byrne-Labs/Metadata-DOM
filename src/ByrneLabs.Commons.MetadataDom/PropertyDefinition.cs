using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition" />
    public class PropertyDefinition : CodeElementWithHandle
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<MethodDefinition> _getter;
        private readonly Lazy<string> _name;
        private readonly Lazy<MethodDefinition> _setter;
        private readonly Lazy<Blob> _signature;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var propertyDefinition = Reader.GetPropertyDefinition(metadataHandle);
            _name = new Lazy<string>(() => AsString(propertyDefinition.Name));
            Attributes = propertyDefinition.Attributes;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(propertyDefinition.Signature)));
            _getter = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(propertyDefinition.GetAccessors().Getter));
            _setter = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(propertyDefinition.GetAccessors().Setter));
            _defaultValue = new Lazy<Constant>(() => GetCodeElement<Constant>(propertyDefinition.GetDefaultValue()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Attributes" />
        public PropertyAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Getter" />
        public MethodDefinition Getter => _getter.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Setter" />
        public MethodDefinition Setter => _setter.Value;

        public Blob Signature => _signature.Value;
    }
}
