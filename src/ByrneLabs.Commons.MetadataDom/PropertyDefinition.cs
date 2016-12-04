﻿using System;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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

        public PropertyAttributes Attributes { get; }

        public Constant DefaultValue => _defaultValue.Value;

        public MethodDefinition Getter => _getter.Value;

        public string Name => _name.Value;

        public MethodDefinition Setter => _setter.Value;

        public Blob Signature => _signature.Value;
    }
}
