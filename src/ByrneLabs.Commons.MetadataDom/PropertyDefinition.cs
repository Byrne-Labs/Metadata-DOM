using System;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition" />
    [PublicAPI]
    public class PropertyDefinition : RuntimeCodeElement, ICodeElementWithHandle<PropertyDefinitionHandle, System.Reflection.Metadata.PropertyDefinition>
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<MethodDefinition> _getter;
        private readonly Lazy<MethodDefinition> _setter;
        private readonly Lazy<Blob> _signature;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetPropertyDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            _getter = GetLazyCodeElementWithHandle<MethodDefinition>(MetadataToken.GetAccessors().Getter);
            _setter = GetLazyCodeElementWithHandle<MethodDefinition>(MetadataToken.GetAccessors().Setter);
            _defaultValue = GetLazyCodeElementWithHandle<Constant>(MetadataToken.GetDefaultValue());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Attributes" />
        public PropertyAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Getter" />
        public MethodDefinition Getter => _getter.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Setter" />
        public MethodDefinition Setter => _setter.Value;

        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public PropertyDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.PropertyDefinition MetadataToken { get; }
    }
}
