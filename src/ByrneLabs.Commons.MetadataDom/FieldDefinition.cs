using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition" />
    public class FieldDefinition : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _declaringType;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly Lazy<string> _name;
        private readonly Lazy<Blob> _signature;

        internal FieldDefinition(FieldDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var fieldDefinition = Reader.GetFieldDefinition(metadataHandle);
            _name = new Lazy<string>(() => AsString(fieldDefinition.Name));
            Attributes = fieldDefinition.Attributes;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(fieldDefinition.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(fieldDefinition.GetCustomAttributes()));
            _defaultValue = new Lazy<Constant>(() => GetCodeElement<Constant>(fieldDefinition.GetDefaultValue()));
            _declaringType = new Lazy<CodeElement>(() => GetCodeElement(fieldDefinition.GetDeclaringType()));
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(fieldDefinition.GetMarshallingDescriptor())));
            Offset = fieldDefinition.GetOffset();
            RelativeVirtualAddress = fieldDefinition.GetRelativeVirtualAddress();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Attributes" />
        public FieldAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDeclaringType" />
        public CodeElement DeclaringType => _declaringType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetOffset" />
        public int Offset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetRelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Signature" />
        public Blob Signature => _signature.Value;
    }
}
