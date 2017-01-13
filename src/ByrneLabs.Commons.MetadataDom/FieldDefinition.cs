using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class FieldDefinition : MemberBase, ICodeElementWithTypedHandle<FieldDefinitionHandle, System.Reflection.Metadata.FieldDefinition>, IContainsSourceCode, IField
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<TypeBase> _fieldType;
        private readonly Lazy<Blob> _marshallingDescriptor;

        internal FieldDefinition(FieldDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetFieldDefinition(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            _declaringType = new Lazy<TypeDefinition>(() => MetadataState.GetCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType()));
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.GetMarshallingDescriptor())));
            Offset = RawMetadata.GetOffset();
            RelativeVirtualAddress = RawMetadata.GetRelativeVirtualAddress();
            _fieldType = new Lazy<TypeBase>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, new TypeBase[] { })));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Attributes" />
        public FieldAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        public bool IsAssembly => Attributes.HasFlag(FieldAttributes.Assembly);

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsFamily => Attributes.HasFlag(FieldAttributes.Family) && !IsPublic;

        public bool IsFamilyAndAssembly => Attributes.HasFlag(FieldAttributes.FamANDAssem) && !IsPublic && !IsAssembly;

        public bool IsFamilyOrAssembly => Attributes.HasFlag(FieldAttributes.FamORAssem) && !IsPublic;

        public bool IsInitOnly => Attributes.HasFlag(FieldAttributes.InitOnly);

        public bool IsLiteral => Attributes.HasFlag(FieldAttributes.Literal);

        public bool IsNotSerialized => Attributes.HasFlag(FieldAttributes.NotSerialized);

        public bool IsPinvokeImpl => Attributes.HasFlag(FieldAttributes.PinvokeImpl);

        public bool IsPrivate => Attributes.HasFlag(FieldAttributes.Private) && !IsAssembly;

        public bool IsPublic => Attributes.HasFlag(FieldAttributes.Public);

        public bool IsSpecialName => Attributes.HasFlag(FieldAttributes.SpecialName);

        public bool IsStatic => Attributes.HasFlag(FieldAttributes.Static);

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public IModule Module { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetOffset" />
        public int Offset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetRelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        public string SourceFile { get; }

        public System.Reflection.Metadata.FieldDefinition RawMetadata { get; }

        public FieldDefinitionHandle MetadataHandle { get; }

        public Document Document { get; }

        public string SourceCode { get; }

        public TypeBase FieldType => _fieldType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDeclaringType" />
        public override TypeBase DeclaringType => _declaringType.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override MemberTypes MemberType => MemberTypes.Field;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Name" />
        public override string Name { get; }

        public override string TextSignature => FullName;
    }
}
