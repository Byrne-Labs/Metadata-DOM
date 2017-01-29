using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
        private Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private Lazy<TypeDefinition> _declaringType;
        private Lazy<ImmutableArray<EventDefinition>> _events;
        private Lazy<ImmutableArray<FieldDefinition>> _fields;
        private Lazy<ImmutableArray<GenericParameter>> _genericParameters;
        private Lazy<ImmutableArray<InterfaceImplementation>> _interfaceImplementations;
        private Lazy<ImmutableArray<MemberBase>> _members;
        private Lazy<ImmutableArray<MethodImplementation>> _methodImplementations;
        private Lazy<ImmutableArray<MethodDefinitionBase>> _methods;
        private Lazy<NamespaceDefinition> _namespaceDefinition;
        private Lazy<ImmutableArray<TypeDefinition>> _nestedTypes;
        private Lazy<ImmutableArray<PropertyDefinition>> _properties;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeDefinition(TypeDefinition baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeDefinition(TypeDefinition genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
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
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclarativeSecurityAttributes" />
        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclaringType" />
        /// <summary>Returns the enclosing type of a specified nested type or null if the type is not nested.</summary>
        public override TypeBase DeclaringType => _declaringType.Value;

        public ImmutableArray<Document> Documents { get; private set; } = ImmutableArray<Document>.Empty;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetEvents" />
        public ImmutableArray<EventDefinition> Events => _events.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetFields" />
        public ImmutableArray<FieldDefinition> Fields => _fields.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetGenericParameters" />
        public ImmutableArray<GenericParameter> GenericTypeParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetInterfaceImplementations" />
        public ImmutableArray<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public bool IsAbstract => (Attributes & TypeAttributes.Abstract) != 0;

        public bool IsAnsiClass => (Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.AnsiClass;

        public bool IsAutoClass => (Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;

        public bool IsAutoLayout => (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.AutoLayout;

        public bool IsClass => (Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class && !IsValueType;

        public bool IsDelegate => "System.Delegate".Equals(BaseType?.FullName) || "System.MulticastDelegate".Equals(BaseType?.FullName);

        public bool IsEnum => "System.Enum".Equals(BaseType?.FullName);

        public bool IsExplicitLayout => (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;

        public override bool IsGenericParameter { get; } = false;

        public override bool IsGenericType => base.IsGenericType || GenericTypeParameters.Any();

        public bool IsGenericTypeDefinition => GenericTypeParameters.Any();

        public bool IsImport => (Attributes & TypeAttributes.Import) != 0;

        public bool IsInterface => (Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;

        public bool IsLayoutSequential => (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;

        public bool IsNestedAssembly => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;

        public bool IsNestedFamANDAssem => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;

        public bool IsNestedFamily => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;

        public bool IsNestedFamORAssem => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem;

        public bool IsNestedPrivate => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;

        public bool IsNestedPublic => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;

        public bool IsNotPublic => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic;

        public bool IsPublic => (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public;

        public bool IsSealed => (Attributes & TypeAttributes.Sealed) != 0;

        public virtual bool IsSerializable => (Attributes & TypeAttributes.Serializable) != 0 || IsEnum || IsDelegate;

        public bool IsSpecialName => (Attributes & TypeAttributes.SpecialName) != 0;

        public bool IsUnicodeClass => (Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;

        public bool IsValueType => IsEnum || "System.ValueType".Equals(BaseType?.FullName);

        public ImmutableArray<Language> Languages { get; private set; } = ImmutableArray<Language>.Empty;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetLayout" />
        public TypeLayout Layout { get; private set; }

        public ImmutableArray<MemberBase> Members => _members.Value;

        public override MemberTypes MemberType => DeclaringType == null ? MemberTypes.TypeInfo : MemberTypes.NestedType;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethodImplementations" />
        public ImmutableArray<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethods" />
        public ImmutableArray<MethodDefinitionBase> Methods => _methods.Value;

        public override string Namespace => NamespaceDefinition == null ? DeclaringType?.Namespace : AsString(RawMetadata.Namespace);

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.NamespaceDefinition" />
        /// <summary>The definition handle of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetNestedTypes" />
        public ImmutableArray<TypeDefinition> NestedTypes => _nestedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetProperties" />
        public ImmutableArray<PropertyDefinition> Properties => _properties.Value;

        internal override string UndecoratedName => AsString(RawMetadata.Name);

        private void Initialize()
        {
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.NamespaceDefinition);
            _methods = MetadataState.GetLazyCodeElements<MethodDefinitionBase>(RawMetadata.GetMethods());
            _methodImplementations = MetadataState.GetLazyCodeElements<MethodImplementation>(RawMetadata.GetMethodImplementations());
            Attributes = RawMetadata.Attributes;
            _baseType = new Lazy<TypeBase>(() =>
            {
                TypeBase baseType;
                if (RawMetadata.BaseType.Kind == HandleKind.TypeSpecification)
                {
                    baseType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.BaseType, this);
                }
                else
                {
                    baseType = (TypeBase)MetadataState.GetCodeElement(RawMetadata.BaseType);
                }
                return baseType;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _events = MetadataState.GetLazyCodeElements<EventDefinition>(RawMetadata.GetEvents());
            _fields = MetadataState.GetLazyCodeElements<FieldDefinition>(RawMetadata.GetFields());
            _genericParameters = new Lazy<ImmutableArray<GenericParameter>>(() =>
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
            _interfaceImplementations = new Lazy<ImmutableArray<InterfaceImplementation>>(() => RawMetadata.GetInterfaceImplementations().Select(interfaceImplementationMetadata => MetadataState.GetCodeElement<InterfaceImplementation>(interfaceImplementationMetadata, this)).ToImmutableArray());
            Layout = RawMetadata.GetLayout();
            _nestedTypes = MetadataState.GetLazyCodeElements<TypeDefinition>(RawMetadata.GetNestedTypes());
            _properties = MetadataState.GetLazyCodeElements<PropertyDefinition>(RawMetadata.GetProperties());
            _members = new Lazy<ImmutableArray<MemberBase>>(() => Methods.Union<MemberBase>(Fields).Union(Events).Union(Properties).Union(NestedTypes).ToImmutableArray());
        }
    }
}
