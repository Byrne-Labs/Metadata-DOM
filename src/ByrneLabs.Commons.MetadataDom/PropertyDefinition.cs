using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: \"{DeclaringType.FullName,nq}.{Name}\"")]
    public class PropertyDefinition : RuntimeCodeElement, IMember, ICodeElementWithHandle<PropertyDefinitionHandle, System.Reflection.Metadata.PropertyDefinition>
    {
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<MethodDefinition> _getMethod;
        private readonly Lazy<MethodDefinition> _setMethod;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal PropertyDefinition(PropertyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetPropertyDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _getMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(MetadataToken.GetAccessors().Getter);
            _setMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(MetadataToken.GetAccessors().Setter);
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(MetadataToken.GetDefaultValue());
            _signature = new Lazy<MethodSignature<TypeBase>>(() => MetadataToken.DecodeSignature(MetadataState.TypeProvider, new GenericContext(((TypeDefinition) DeclaringType).GenericTypeParameters, new TypeBase[] { })));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Attributes" />
        public PropertyAttributes Attributes { get; }

        public bool CanRead => GetMethod != null;

        public bool CanWrite => SetMethod != null;

        public IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Getter" />
        public MethodDefinition GetMethod => _getMethod.Value;

        public bool IsSpecialName => Attributes.HasFlag(PropertyAttributes.SpecialName);

        public TypeBase PropertyType => Signature.ReturnType;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyAccessors.Setter" />
        public MethodDefinition SetMethod => _setMethod.Value;

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public PropertyDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.PropertyDefinition MetadataToken { get; }

        public TypeBase DeclaringType => GetMethod?.DeclaringType ?? SetMethod?.DeclaringType;

        public string FullName => $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType { get; } = MemberTypes.Property;

        /// <inheritdoc cref="System.Reflection.Metadata.PropertyDefinition.Name" />
        public string Name { get; }

        public string TextSignature => $"{PropertyType.TextSignature} {FullName}";
    }
}
