using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public partial class PropertyDefinition : PropertyInfo, IManagedCodeElement
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly MethodSignature<TypeBase> _signature;
        private Lazy<ImmutableArray<CustomAttributeData>> _customAttributes;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<PropertyDefinition>(metadataHandle);
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            RawMetadata = MetadataState.AssemblyReader.GetPropertyDefinition(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            if (!RawMetadata.GetAccessors().Getter.IsNil)
            {
                GetMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Getter);
            }
            if (!RawMetadata.GetAccessors().Setter.IsNil)
            {
                SetMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Setter);
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            var declaringType = (TypeDefinition) (GetMethod?.DeclaringType ?? SetMethod?.DeclaringType);
            _signature = RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, declaringType.GenericTypeParameters, null));
        }

        public override PropertyAttributes Attributes => RawMetadata.Attributes;

        public override bool CanRead => GetMethod != null;

        public override bool CanWrite => SetMethod != null;

        public override TypeToExpose DeclaringType => GetMethod?.DeclaringType ?? SetMethod?.DeclaringType;

        public override ConstantInfo DefaultValue => _defaultValue.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}" + (IsIndexer ? $"[{string.Join(", ", _signature.ParameterTypes.Select(parameterType => parameterType.FullName))}]" : string.Empty);

        public override sealed MethodInfoToExpose GetMethod { get; }

        public override bool IsIndexer => _signature.ParameterTypes.Any() && "Item".Equals(Name);

        public override bool IsSpecialName => Attributes.HasFlag(PropertyAttributes.SpecialName);

        public override MemberTypes MemberType { get; } = MemberTypes.Property;

        public PropertyDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override TypeToExpose PropertyType => _signature.ReturnType;

        public System.Reflection.Metadata.PropertyDefinition RawMetadata { get; }

        public override TypeToExpose ReflectedType => null;

        public override sealed MethodInfoToExpose SetMethod { get; }

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override MethodInfoToExpose[] GetAccessors(bool nonPublic) => new[] { GetMethod, SetMethod }.Where(method => method != null).Where(method => method.IsPublic || nonPublic).ToArray();

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override MethodInfoToExpose GetGetMethod(bool nonPublic) => nonPublic || GetMethod?.IsPublic == true ? GetMethod : null;

        public override ParameterInfoToExpose[] GetIndexParameters() => throw new NotImplementedException();

        public override MethodInfoToExpose GetSetMethod(bool nonPublic) => nonPublic || SetMethod?.IsPublic == true ? SetMethod : null;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class PropertyDefinition
    {
        public override object[] GetCustomAttributes(bool inherit) => CustomAttributeData.GetCustomAttributes(this, inherit);

        public override object[] GetCustomAttributes(TypeToExpose attributeType, bool inherit) => CustomAttributeData.GetCustomAttributes(this, attributeType, inherit);

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => CustomAttributeData.IsDefined(this, attributeType, inherit);
    }
#else
    public partial class PropertyDefinition
    {
    }
#endif
}
