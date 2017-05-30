using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public partial class FieldDefinition : FieldInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
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

        public override TypeToExpose DeclaringType => _declaringType.Value;

        public ConstantInfo DefaultValue => _defaultValue.Value;

        public override TypeToExpose FieldType => _fieldType.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override MemberTypes MemberType => MemberTypes.Field;

        public FieldDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public int Offset { get; }

        public System.Reflection.Metadata.FieldDefinition RawMetadata { get; }

        public override TypeToExpose ReflectedType => throw new NotSupportedException();

        public int RelativeVirtualAddress { get; }

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override object GetRawConstantValue() => DefaultValue.Value;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class FieldDefinition
    {
        public override RuntimeFieldHandle FieldHandle => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override object GetValue(object obj) => throw new NotSupportedException();

        public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture) => throw new NotSupportedException();
    }
#else
    public partial class FieldDefinition
    {
    }
#endif
}
