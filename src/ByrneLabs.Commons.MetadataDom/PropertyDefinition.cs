using System;
using System.Collections.Generic;
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
        private readonly Lazy<MethodDefinition> _getMethod;
        private readonly Lazy<MethodDefinition> _setMethod;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetPropertyDefinition(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _getMethod = new Lazy<MethodDefinition>(() =>
            {
                var getMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Getter);
                if (getMethod != null)
                {
                    getMethod.RelatedProperty = this;
                }
                return getMethod;
            });
            _setMethod = new Lazy<MethodDefinition>(() =>
            {
                var setMethod = MetadataState.GetCodeElement<MethodDefinition>(RawMetadata.GetAccessors().Setter);
                if (setMethod != null)
                {
                    setMethod.RelatedProperty = this;
                }
                return setMethod;
            });
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(((TypeDefinition) DeclaringType).GenericTypeParameters, new TypeBase[] { })));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Attributes" />
        public PropertyAttributes Attributes { get; }

        public bool CanRead => GetMethod != null;

        public bool CanWrite => SetMethod != null;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Getter" />
        public MethodDefinition GetMethod => _getMethod.Value;

        public bool IsSpecialName => Attributes.HasFlag(PropertyAttributes.SpecialName);

        public TypeBase PropertyType => Signature.ReturnType;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Setter" />
        public MethodDefinition SetMethod => _setMethod.Value;

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        public System.Reflection.Metadata.PropertyDefinition RawMetadata { get; }

        public PropertyDefinitionHandle MetadataHandle { get; }

        public override TypeBase DeclaringType => GetMethod?.DeclaringType ?? SetMethod?.DeclaringType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}" + ("Item".Equals(Name) && GetMethod != null && GetMethod.Parameters.Count() > 0 ? $"[{string.Join(", ", GetMethod.Parameters.Select(parameter => parameter.ParameterType.FullName))}]" : string.Empty);

        public override MemberTypes MemberType { get; } = MemberTypes.Property;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Name" />
        public override string Name { get; }

        public override string TextSignature => FullName;
    }
}
