using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public FieldAttributes Attributes { get; }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement DeclaringType => _declaringType.Value;

        public Constant DefaultValue => _defaultValue.Value;

        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public string Name => _name.Value;

        public int Offset { get; }

        public int RelativeVirtualAddress { get; }

        public Blob Signature => _signature.Value;
    }
}
