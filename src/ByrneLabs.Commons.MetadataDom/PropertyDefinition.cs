using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class PropertyDefinition : MemberBase, ICodeElementWithTypedHandle<PropertyDefinitionHandle, System.Reflection.Metadata.PropertyDefinition>
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly MethodSignature<TypeBase> _signature;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetPropertyDefinition(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            if (!RawMetadata.GetAccessors().Getter.IsNil)
            {
                GetMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Getter);
                GetMethod.RelatedProperty = this;
            }
            if (!RawMetadata.GetAccessors().Setter.IsNil)
            {
                SetMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Setter);
                SetMethod.RelatedProperty = this;
            }
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            var declaringType = (TypeDefinition)(GetMethod?.DeclaringType ?? SetMethod?.DeclaringType);
            _signature = RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, declaringType.GenericTypeParameters, null));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Attributes" />
        public PropertyAttributes Attributes { get; }

        public bool CanRead => GetMethod != null;

        public bool CanWrite => SetMethod != null;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType => GetMethod?.DeclaringType ?? SetMethod?.DeclaringType;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}" + (IsIndexer ? $"[{string.Join(", ", _signature.ParameterTypes.Select(parameterType => parameterType.FullName))}]" : string.Empty);

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Getter" />
        public MethodDefinition GetMethod { get; }

        public bool IsSpecialName => Attributes.HasFlag(PropertyAttributes.SpecialName);

        public override MemberTypes MemberType { get; } = MemberTypes.Property;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Name" />
        public override string Name { get; }

        public TypeBase PropertyType => _signature.ReturnType;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Setter" />
        public MethodDefinition SetMethod { get; }

        public override string TextSignature => FullName;

        public bool IsIndexer => _signature.ParameterTypes.Any() && "Item".Equals(Name);

        public System.Reflection.Metadata.PropertyDefinition RawMetadata { get; }

        public PropertyDefinitionHandle MetadataHandle { get; }
    }
}
