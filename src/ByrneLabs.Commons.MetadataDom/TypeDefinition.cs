using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition" />
    //[PublicAPI]
    public class TypeDefinition : TypeBase<TypeDefinition, TypeDefinitionHandle, System.Reflection.Metadata.TypeDefinition>
    {
        private Lazy<TypeBase> _baseType;
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private Lazy<TypeDefinition> _declaringType;
        private Lazy<IEnumerable<EventDefinition>> _events;
        private Lazy<IEnumerable<FieldDefinition>> _fields;
        private Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private Lazy<IEnumerable<InterfaceImplementation>> _interfaceImplementations;
        private Lazy<IEnumerable<IMember>> _members;
        private Lazy<IEnumerable<MethodImplementation>> _methodImplementations;
        private Lazy<IEnumerable<IMethodBase>> _methods;
        private Lazy<NamespaceDefinition> _namespaceDefinition;
        private Lazy<IEnumerable<TypeDefinition>> _nestedTypes;
        private Lazy<IEnumerable<PropertyDefinition>> _properties;

        internal TypeDefinition(TypeDefinition baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal TypeDefinition(TypeDefinition genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal TypeDefinition(TypeDefinitionHandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            Initialize();
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Attributes" />
        public TypeAttributes Attributes { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.BaseType" />
        /// <summary>The base type of the type definition: either <see cref="TypeSpecification" />, <see cref="TypeReference" /> or <see cref="TypeDefinition" />.</summary>
        public TypeBase BaseType => _baseType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclaringType" />
        /// <summary>Returns the enclosing type of a specified nested type or null if the type is not nested.</summary>
        public override TypeBase DeclaringType => _declaringType.Value;

        public IEnumerable<Document> Documents { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetEvents" />
        public IEnumerable<EventDefinition> Events => _events.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetFields" />
        public IEnumerable<IField> Fields => _fields.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetGenericParameters" />
        public IEnumerable<GenericParameter> GenericTypeParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetInterfaceImplementations" />
        public IEnumerable<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public bool IsAbstract => Attributes.HasFlag(TypeAttributes.Abstract);

        public bool IsAnsiClass => Attributes.HasFlag(TypeAttributes.AnsiClass);

        public bool IsAutoClass => Attributes.HasFlag(TypeAttributes.AutoClass);

        public bool IsAutoLayout => Attributes.HasFlag(TypeAttributes.AutoLayout);

        public bool IsClass => Attributes.HasFlag(TypeAttributes.Class);

        public bool IsEnum => "System.Enum".Equals(BaseType?.FullName);

        public bool IsExplicitLayout => Attributes.HasFlag(TypeAttributes.ExplicitLayout);

        public override bool IsGenericParameter { get; } = false;

        public bool IsGenericTypeDefinition => GenericTypeParameters.Any();

        public bool IsImport => Attributes.HasFlag(TypeAttributes.Import);

        public bool IsInterface => Attributes.HasFlag(TypeAttributes.Interface);

        public bool IsNestedAssembly => Attributes.HasFlag(TypeAttributes.NestedAssembly);

        public bool IsNestedFamANDAssem => Attributes.HasFlag(TypeAttributes.NestedFamANDAssem);

        public bool IsNestedFamily => Attributes.HasFlag(TypeAttributes.NestedFamily);

        public bool IsNestedFamORAssem => Attributes.HasFlag(TypeAttributes.NestedFamORAssem);

        public bool IsNestedPrivate => Attributes.HasFlag(TypeAttributes.NestedPrivate);

        public bool IsNestedPublic => Attributes.HasFlag(TypeAttributes.NestedPublic);

        public bool IsNotPublic => Attributes.HasFlag(TypeAttributes.NotPublic);

        public bool IsPublic => Attributes.HasFlag(TypeAttributes.Public);

        public bool IsSealed => Attributes.HasFlag(TypeAttributes.Sealed);

        public bool IsSerializable => Attributes.HasFlag(TypeAttributes.Serializable);

        public bool IsSpecialName => Attributes.HasFlag(TypeAttributes.SpecialName);

        public bool IsUnicodeClass => Attributes.HasFlag(TypeAttributes.UnicodeClass);

        public bool IsValueType => "System.ValueType".Equals(BaseType?.FullName);

        public IEnumerable<Language> Languages { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetLayout" />
        public TypeLayout Layout { get; private set; }

        public IEnumerable<IMember> Members => _members.Value;

        public override MemberTypes MemberType => DeclaringType == null ? MemberTypes.TypeInfo : MemberTypes.NestedType;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethodImplementations" />
        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethods" />
        public IEnumerable<IMethodBase> Methods => _methods.Value;

        public override string Namespace => NamespaceDefinition == null ? DeclaringType?.Namespace : AsString(RawMetadata.Namespace);

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.NamespaceDefinition" />
        /// <summary>The definition handle of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetNestedTypes" />
        public IEnumerable<TypeBase> NestedTypes => _nestedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetProperties" />
        public IEnumerable<PropertyDefinition> Properties => _properties.Value;

        internal override string UndecoratedName => AsString(RawMetadata.Name);

        private void Initialize()
        {
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.NamespaceDefinition);
            _methods = MetadataState.GetLazyCodeElements<IMethodBase>(RawMetadata.GetMethods());
            _methodImplementations = MetadataState.GetLazyCodeElements<MethodImplementation>(RawMetadata.GetMethodImplementations());
            Attributes = RawMetadata.Attributes;
            _baseType = new Lazy<TypeBase>(() =>
            {
                var baseType = (TypeBase) MetadataState.GetCodeElement(RawMetadata.BaseType);
                var typeSpecification = baseType as TypeSpecification;
                if (typeSpecification != null)
                {
                    typeSpecification.ParentTypeDefinition = this;
                }
                return baseType;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _events = MetadataState.GetLazyCodeElements<EventDefinition>(RawMetadata.GetEvents());
            _fields = MetadataState.GetLazyCodeElements<FieldDefinition>(RawMetadata.GetFields());
            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                var index = 0;
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.SetDeclaringType(this);
                    genericParameter.Index = index++;
                }

                return genericParameters;
            });
            _interfaceImplementations = new Lazy<IEnumerable<InterfaceImplementation>>(() =>
            {
                var interfaceImplementations = MetadataState.GetCodeElements<InterfaceImplementation>(RawMetadata.GetInterfaceImplementations());
                foreach (var interfaceImplementation in interfaceImplementations)
                {
                    interfaceImplementation.ImplementingType = this;
                }

                return interfaceImplementations;
            });
            Layout = RawMetadata.GetLayout();
            _nestedTypes = MetadataState.GetLazyCodeElements<TypeDefinition>(RawMetadata.GetNestedTypes());
            _properties = MetadataState.GetLazyCodeElements<PropertyDefinition>(RawMetadata.GetProperties());
            _members = new Lazy<IEnumerable<IMember>>(() => Methods.Union<IMember>(Fields).Union(Events).Union(Properties).Union(NestedTypes).ToList());
        }
    }
}
