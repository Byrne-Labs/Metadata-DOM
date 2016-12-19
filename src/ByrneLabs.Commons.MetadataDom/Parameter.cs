using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Parameter" />
    //[PublicAPI]
    public class Parameter : RuntimeCodeElement, ICodeElementWithHandle<ParameterHandle, System.Reflection.Metadata.Parameter>, IParameter
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
            Position = MetadataToken.SequenceNumber;
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(MetadataToken.GetDefaultValue());
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.GetMarshallingDescriptor())));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Attributes" />
        public ParameterAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        public bool IsLcid => Attributes.HasFlag(ParameterAttributes.Lcid);

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ParameterHandle MetadataHandle { get; }

        public System.Reflection.Metadata.Parameter MetadataToken { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public bool HasDefaultValue => Attributes.HasFlag(ParameterAttributes.HasDefault);

        public bool IsIn => Attributes.HasFlag(ParameterAttributes.In);

        public bool IsOptional => Attributes.HasFlag(ParameterAttributes.Optional);

        public bool IsOut => Attributes.HasFlag(ParameterAttributes.Out);

        public bool IsRetval => Attributes.HasFlag(ParameterAttributes.Retval);

        public IMember Member { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Name" />
        public string Name { get; }

        public TypeBase ParameterType { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.SequenceNumber" />
        public int Position { get; }

        public string TextSignature => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;
    }
}
