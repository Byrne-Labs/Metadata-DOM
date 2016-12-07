using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Parameter" />
    public class Parameter : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly Lazy<string> _name;

        internal Parameter(ParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var parameter = Reader.GetParameter(metadataHandle);
            _name = new Lazy<string>(() => AsString(parameter.Name));
            Attributes = parameter.Attributes;
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(parameter.GetCustomAttributes()));
            SequenceNumber = parameter.SequenceNumber;
            _defaultValue = new Lazy<Constant>(() => GetCodeElement<Constant>(parameter.GetDefaultValue()));
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(parameter.GetMarshallingDescriptor())));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Attributes" />
        public ParameterAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.SequenceNumber" />
        public int SequenceNumber { get; }
    }
}
