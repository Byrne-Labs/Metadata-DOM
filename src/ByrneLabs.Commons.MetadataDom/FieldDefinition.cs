using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition" />
    [PublicAPI]
    public class FieldDefinition : RuntimeCodeElement, ICodeElementWithHandle<FieldDefinitionHandle, System.Reflection.Metadata.FieldDefinition>, IContainsSourceCode
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeBase> _declaringType;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<Blob> _signature;

        internal FieldDefinition(FieldDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetFieldDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _defaultValue = GetLazyCodeElementWithHandle<Constant>(MetadataToken.GetDefaultValue());
            _declaringType = GetLazyCodeElementWithHandle<TypeBase>(MetadataToken.GetDeclaringType());
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.GetMarshallingDescriptor())));
            Offset = MetadataToken.GetOffset();
            RelativeVirtualAddress = MetadataToken.GetRelativeVirtualAddress();
            _methodBody = new Lazy<MethodBody>(() => MetadataToken.GetRelativeVirtualAddress() == 0 ? null : GetCodeElementWithoutHandle<MethodBody>(MetadataToken.GetRelativeVirtualAddress()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Attributes" />
        public FieldAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDeclaringType" />
        public TypeBase DeclaringType => _declaringType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetOffset" />
        public int Offset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetRelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Signature" />
        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public FieldDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.FieldDefinition MetadataToken { get; }

        public Document Document { get; }

        public string SourceCode { get; }

        public string SourceFile { get; }
    }
}
