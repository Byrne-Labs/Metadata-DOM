using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Parameter" />
    [PublicAPI]
    public class Parameter : RuntimeCodeElement, ICodeElementWithHandle<ParameterHandle, System.Reflection.Metadata.Parameter>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;

        internal Parameter(ParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetParameter(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            SequenceNumber = MetadataToken.SequenceNumber;
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(MetadataToken.GetDefaultValue());
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.GetMarshallingDescriptor())));
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
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.SequenceNumber" />
        public int SequenceNumber { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ParameterHandle MetadataHandle { get; }

        public System.Reflection.Metadata.Parameter MetadataToken { get; }
    }
}
