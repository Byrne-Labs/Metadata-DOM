using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class PropertyDefinition : PropertyInfo, IManagedCodeElement
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly MethodSignature<TypeBase> _signature;
        private readonly Lazy<IEnumerable<CustomAttributeData>> _customAttributes;

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

        public override Type DeclaringType => GetMethod?.DeclaringType ?? SetMethod?.DeclaringType;

        public override ConstantInfo DefaultValue => _defaultValue.Value;

        public override string FullName => $"{PropertyType} {Name}" + (IsIndexer ? $"[{string.Join(", ", _signature.ParameterTypes.Select(parameterType => parameterType.FullName))}]" : string.Empty);

        public override sealed System.Reflection.MethodInfo GetMethod { get; }

        public override bool IsIndexer => _signature.ParameterTypes.Any() && "Item".Equals(Name);

        public override MemberTypes MemberType { get; } = MemberTypes.Property;

        public PropertyDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override Type PropertyType => _signature.ReturnType;

        public System.Reflection.Metadata.PropertyDefinition RawMetadata { get; }

        public override Type ReflectedType => null;

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => throw new NotImplementedException();

        public override sealed System.Reflection.MethodInfo SetMethod { get; }

        public override string SourceCode => throw new NotImplementedException();

        public override string TextSignature => $"{DeclaringType.FullName}.{Name}" + (IsIndexer ? $"[{string.Join(", ", _signature.ParameterTypes.Select(parameterType => parameterType.FullName))}]" : string.Empty);

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo[] GetAccessors(bool nonPublic) => new[] { GetMethod, SetMethod }.Where(method => method != null).Where(method => method.IsPublic || nonPublic).ToArray();

        public override object[] GetCustomAttributes(bool inherit) => CustomAttributeData.GetCustomAttributes(this, inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => CustomAttributeData.GetCustomAttributes(this, attributeType, inherit);

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override System.Reflection.MethodInfo GetGetMethod(bool nonPublic) => nonPublic || GetMethod?.IsPublic == true ? GetMethod : null;

        public override System.Reflection.ParameterInfo[] GetIndexParameters() => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.MethodInfo GetSetMethod(bool nonPublic) => nonPublic || SetMethod?.IsPublic == true ? SetMethod : null;

        public override bool IsDefined(Type attributeType, bool inherit) => CustomAttributeData.IsDefined(this, attributeType, inherit);
    }
}
