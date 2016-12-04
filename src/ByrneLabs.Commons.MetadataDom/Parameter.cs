using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class Parameter : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly Lazy<string> _name;

        internal Parameter(ParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var parameter = Reader.GetParameter(metadataHandle);
            _name = new Lazy<string>(() => AsString(parameter.Name));
            Attributes = parameter.Attributes;
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(parameter.GetCustomAttributes()));
            SequenceNumber = parameter.SequenceNumber;
            _defaultValue = new Lazy<Constant>(() => GetCodeElement<Constant>(parameter.GetDefaultValue()));
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(parameter.GetMarshallingDescriptor())));
        }

        public ParameterAttributes Attributes { get; }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public Constant DefaultValue => _defaultValue.Value;

        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public string Name => _name.Value;

        public int SequenceNumber { get; }
    }
}
