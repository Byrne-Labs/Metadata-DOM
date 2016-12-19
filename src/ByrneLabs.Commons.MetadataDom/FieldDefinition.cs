using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {DeclaringType.FullName,nq}.{Name}")]
    public class FieldDefinition : RuntimeCodeElement, ICodeElementWithHandle<FieldDefinitionHandle, System.Reflection.Metadata.FieldDefinition>, IContainsSourceCode, IField
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<TypeBase> _fieldType;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly Lazy<MethodBody> _methodBody;

        internal FieldDefinition(FieldDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetFieldDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(MetadataToken.GetDefaultValue());
            _declaringType = new Lazy<TypeDefinition>(() => MetadataState.GetCodeElement<TypeDefinition>(MetadataToken.GetDeclaringType()));
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.GetMarshallingDescriptor())));
            Offset = MetadataToken.GetOffset();
            RelativeVirtualAddress = MetadataToken.GetRelativeVirtualAddress();
            _fieldType = new Lazy<TypeBase>(() => MetadataToken.DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, new TypeBase[] { })));
            _methodBody = new Lazy<MethodBody>(() => MetadataToken.GetRelativeVirtualAddress() == 0 ? null : MetadataState.GetCodeElement<MethodBody>(MetadataToken.GetRelativeVirtualAddress()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Attributes" />
        public FieldAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        public bool IsAssembly => Attributes.HasFlag(FieldAttributes.Assembly);

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsFamily => Attributes.HasFlag(FieldAttributes.Family);

        public bool IsFamilyAndAssembly => Attributes.HasFlag(FieldAttributes.FamANDAssem);

        public bool IsFamilyOrAssembly => Attributes.HasFlag(FieldAttributes.FamORAssem);

        public bool IsInitOnly => Attributes.HasFlag(FieldAttributes.InitOnly);

        public bool IsLiteral => Attributes.HasFlag(FieldAttributes.Literal);

        public bool IsNotSerialized => Attributes.HasFlag(FieldAttributes.NotSerialized);

        public bool IsPinvokeImpl => Attributes.HasFlag(FieldAttributes.PinvokeImpl);

        public bool IsPrivate => Attributes.HasFlag(FieldAttributes.Private);

        public bool IsPublic => Attributes.HasFlag(FieldAttributes.Public);

        public bool IsSpecialName => Attributes.HasFlag(FieldAttributes.SpecialName);

        public bool IsStatic => Attributes.HasFlag(FieldAttributes.Static);

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public MethodBody MethodBody => _methodBody.Value;

        public IModule Module { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetOffset" />
        public int Offset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetRelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        public string SourceFile { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public FieldDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.FieldDefinition MetadataToken { get; }

        public Document Document { get; }

        public string SourceCode { get; }

        public TypeBase FieldType => _fieldType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.GetDeclaringType" />
        public TypeBase DeclaringType => _declaringType.Value;

        public string FullName => $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType => MemberTypes.Field;

        /// <inheritdoc cref="System.Reflection.Metadata.FieldDefinition.Name" />
        public string Name { get; }

        public string TextSignature => $"{FieldType.FullName} {FullName}";
    }
}
