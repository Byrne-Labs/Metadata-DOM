using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition" />
    [PublicAPI]
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
        private Lazy<IEnumerable<MethodImplementation>> _methodImplementations;
        private Lazy<IEnumerable<MethodDefinitionBase>> _methods;
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

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Attributes" />
        public TypeAttributes Attributes { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.BaseType" />
        /// <summary>The base type of the type definition: either <see cref="TypeSpecification" />, <see cref="TypeReference" /> or <see cref="TypeDefinition" />.</summary>
        public TypeBase BaseType => _baseType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclaringType" />
        /// <summary>Returns the enclosing type of a specified nested type or null if the type is not nested.</summary>
        public TypeDefinition DeclaringType => _declaringType.Value;

        public IEnumerable<Document> Documents { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetEvents" />
        public IEnumerable<EventDefinition> Events => _events.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetFields" />
        public IEnumerable<FieldDefinition> Fields => _fields.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetGenericParameters" />
        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetInterfaceImplementations" />
        public IEnumerable<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public IEnumerable<Language> Languages { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetLayout" />
        public TypeLayout Layout { get; private set; }


        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethodImplementations" />
        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethods" />
        public IEnumerable<MethodDefinitionBase> Methods => _methods.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.NamespaceDefinition" />
        /// <summary>The definition handle of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetNestedTypes" />
        public IEnumerable<TypeDefinition> NestedTypes => _nestedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetProperties" />
        public IEnumerable<PropertyDefinition> Properties => _properties.Value;

        private void Initialize()
        {
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(MetadataToken.NamespaceDefinition);
            _methods = new Lazy<IEnumerable<MethodDefinitionBase>>(() => MetadataState.GetCodeElements(MetadataToken.GetMethods()).Cast<MethodDefinitionBase>());
            _methodImplementations = MetadataState.GetLazyCodeElements<MethodImplementation>(MetadataToken.GetMethodImplementations());
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _baseType = new Lazy<TypeBase>(() => (TypeBase)MetadataState.GetCodeElement(MetadataToken.BaseType));
            Namespace = AsString(MetadataToken.Namespace);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(MetadataToken.GetDeclaringType());
            _events = MetadataState.GetLazyCodeElements<EventDefinition>(MetadataToken.GetEvents());
            _fields = MetadataState.GetLazyCodeElements<FieldDefinition>(MetadataToken.GetFields());
            _genericParameters = MetadataState.GetLazyCodeElements<GenericParameter>(MetadataToken.GetGenericParameters());
            _interfaceImplementations = MetadataState.GetLazyCodeElements<InterfaceImplementation>(MetadataToken.GetInterfaceImplementations());
            Layout = MetadataToken.GetLayout();
            _nestedTypes = MetadataState.GetLazyCodeElements<TypeDefinition>(MetadataToken.GetNestedTypes());
            _properties = MetadataState.GetLazyCodeElements<PropertyDefinition>(MetadataToken.GetProperties());

            IsAbstract = Attributes.HasFlag(TypeAttributes.Abstract);
            IsClass = Attributes.HasFlag(TypeAttributes.Class);
            IsEnum = "System.Enum".Equals(BaseType?.FullName);
            IsGenericTypeDefinition = GenericParameters.Any();
            IsImport = Attributes.HasFlag(TypeAttributes.Import);
            IsInterface = Attributes.HasFlag(TypeAttributes.Interface);
            IsNestedAssembly = Attributes.HasFlag(TypeAttributes.NestedAssembly);
            IsNestedFamANDAssem = Attributes.HasFlag(TypeAttributes.NestedFamANDAssem);
            IsNestedFamily = Attributes.HasFlag(TypeAttributes.NestedFamily);
            IsNestedFamORAssem = Attributes.HasFlag(TypeAttributes.NestedFamORAssem);
            IsNestedPrivate = Attributes.HasFlag(TypeAttributes.NestedPrivate);
            IsNestedPublic = Attributes.HasFlag(TypeAttributes.NestedPublic);
            IsNotPublic = Attributes.HasFlag(TypeAttributes.NotPublic);
            IsPublic = Attributes.HasFlag(TypeAttributes.Public);
            FullName = (IsNested && BaseType != null ? BaseType.FullName + "." : string.Empty) + (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".") + Name;
        }
    }
}
