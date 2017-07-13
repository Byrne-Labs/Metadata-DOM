using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class FieldDefinition : FieldInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<TypeBase> _fieldType;

        internal FieldDefinition(FieldDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<FieldDefinition>(metadataHandle);
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            RawMetadata = MetadataState.AssemblyReader.GetFieldDefinition(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            _declaringType = new Lazy<TypeDefinition>(() => MetadataState.GetCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType()));
            Offset = RawMetadata.GetOffset();
            RelativeVirtualAddress = RawMetadata.GetRelativeVirtualAddress();
            _fieldType = new Lazy<TypeBase>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, null)));
        }

        public override FieldAttributes Attributes => RawMetadata.Attributes;

        public override Type DeclaringType => _declaringType.Value;

        public ConstantInfo DefaultValue => _defaultValue.Value;

        public override RuntimeFieldHandle FieldHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override Type FieldType => _fieldType.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override string FullTextSignature => FullName;

        public override MemberTypes MemberType => MemberTypes.Field;

        public FieldDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public int Offset { get; }

        public System.Reflection.Metadata.FieldDefinition RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public int RelativeVirtualAddress { get; }

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => throw NotSupportedHelper.FutureVersion();

        public override string SourceCode => throw NotSupportedHelper.FutureVersion();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override object GetRawConstantValue() => DefaultValue.Value;

        public override object GetValue(object obj) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();
    }
}
