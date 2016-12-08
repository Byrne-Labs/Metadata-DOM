using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition" />
    [DebuggerDisplay("{Namespace}.{Name}")]
    [PublicAPI]
    public class TypeDefinition : CodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _baseType;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IEnumerable<EventDefinition>> _events;
        private readonly Lazy<IEnumerable<FieldDefinition>> _fields;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<IEnumerable<InterfaceImplementation>> _interfaceImplementations;
        private readonly Lazy<IEnumerable<MethodImplementation>> _methodImplementations;
        private readonly Lazy<IEnumerable<MethodDefinitionBase>> _methods;
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;
        private readonly Lazy<IEnumerable<TypeDefinition>> _nestedTypes;
        private readonly Lazy<IEnumerable<PropertyDefinition>> _properties;

        internal TypeDefinition(TypeDefinitionHandle typeDefinitionHandle, MetadataState metadataState) : base(typeDefinitionHandle, metadataState)
        {
            var typeDefinition = Reader.GetTypeDefinition(typeDefinitionHandle);
            _namespaceDefinition = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(typeDefinition.NamespaceDefinition));
            _methods = new Lazy<IEnumerable<MethodDefinitionBase>>(() => GetCodeElements<MethodDefinitionBase>(typeDefinition.GetMethods()));
            _methodImplementations = new Lazy<IEnumerable<MethodImplementation>>(() => GetCodeElements<MethodImplementation>(typeDefinition.GetMethodImplementations()));
            _name = new Lazy<string>(() => AsString(typeDefinition.Name));
            Attributes = typeDefinition.Attributes;
            _baseType = new Lazy<CodeElement>(() => GetCodeElement(typeDefinition.BaseType));
            _namespace = new Lazy<string>(() => AsString(typeDefinition.Namespace));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(typeDefinition.GetCustomAttributes()));
            _declarativeSecurityAttributes = new Lazy<IEnumerable<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(typeDefinition.GetDeclarativeSecurityAttributes()));
            _declaringType = new Lazy<TypeDefinition>(() => GetCodeElement<TypeDefinition>(typeDefinition.GetDeclaringType()));
            _events = new Lazy<IEnumerable<EventDefinition>>(() => GetCodeElements<EventDefinition>(typeDefinition.GetEvents()));
            _fields = new Lazy<IEnumerable<FieldDefinition>>(() => GetCodeElements<FieldDefinition>(typeDefinition.GetFields()));
            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() => GetCodeElements<GenericParameter>(typeDefinition.GetGenericParameters()));
            _interfaceImplementations = new Lazy<IEnumerable<InterfaceImplementation>>(() => GetCodeElements<InterfaceImplementation>(typeDefinition.GetInterfaceImplementations()));
            Layout = typeDefinition.GetLayout();
            _nestedTypes = new Lazy<IEnumerable<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(typeDefinition.GetNestedTypes()));
            _properties = new Lazy<IEnumerable<PropertyDefinition>>(() => GetCodeElements<PropertyDefinition>(typeDefinition.GetProperties()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Attributes" />
        public TypeAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.BaseType" />
        /// <summary>The base type of the type definition: either
        ///     <see cref="T:System.Reflection.Metadata.TypeSpecification" />, <see cref="T:System.Reflection.Metadata.TypeReference" /> or
        ///     <see cref="T:System.Reflection.Metadata.TypeDefinition" />.</summary>
        public CodeElement BaseType => _baseType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetDeclaringType" />
        /// <summary>Returns the enclosing type of a specified nested type or null if the type is not nested.</summary>
        public TypeDefinition DeclaringType => _declaringType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetEvents" />
        public IEnumerable<EventDefinition> Events => _events.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetFields" />
        public IEnumerable<FieldDefinition> Fields => _fields.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetGenericParameters" />
        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetInterfaceImplementations" />
        public IEnumerable<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetLayout" />
        public TypeLayout Layout { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethodImplementations" />
        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethods" />
        public IEnumerable<MethodDefinitionBase> Methods => _methods.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Namespace" />
        /// <summary>Full name of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public string Namespace => _namespace.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.NamespaceDefinition" />
        /// <summary>The definition handle of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetNestedTypes" />
        public IEnumerable<TypeDefinition> NestedTypes => _nestedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetProperties" />
        public IEnumerable<PropertyDefinition> Properties => _properties.Value;
    }
}
