using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class TypeDefinition : CodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _baseType;
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<IReadOnlyList<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IReadOnlyList<EventDefinition>> _events;
        private readonly Lazy<IReadOnlyList<FieldDefinition>> _fields;
        private readonly Lazy<IReadOnlyList<GenericParameter>> _genericParameters;
        private readonly Lazy<IReadOnlyList<InterfaceImplementation>> _interfaceImplementations;
        private readonly Lazy<IReadOnlyList<MethodImplementation>> _methodImplementations;
        private readonly Lazy<IReadOnlyList<MethodDefinition>> _methods;
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;
        private readonly Lazy<IReadOnlyList<TypeDefinition>> _nestedTypes;
        private readonly Lazy<IReadOnlyList<PropertyDefinition>> _properties;

        internal TypeDefinition(TypeDefinitionHandle typeDefinitionHandle, MetadataState metadataState) : base(typeDefinitionHandle, metadataState)
        {
            var typeDefinition = Reader.GetTypeDefinition(typeDefinitionHandle);
            _namespaceDefinition = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(typeDefinition.NamespaceDefinition));
            _methods = new Lazy<IReadOnlyList<MethodDefinition>>(() => GetCodeElements<MethodDefinition>(typeDefinition.GetMethods()));
            _methodImplementations = new Lazy<IReadOnlyList<MethodImplementation>>(() => GetCodeElements<MethodImplementation>(typeDefinition.GetMethodImplementations()));
            _name = new Lazy<string>(() => AsString(typeDefinition.Name));
            Attributes = typeDefinition.Attributes;
            _baseType = new Lazy<CodeElement>(() => GetCodeElement(typeDefinition.BaseType));
            _namespace = new Lazy<string>(() => AsString(typeDefinition.Namespace));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(typeDefinition.GetCustomAttributes()));
            _declarativeSecurityAttributes = new Lazy<IReadOnlyList<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(typeDefinition.GetDeclarativeSecurityAttributes()));
            _declaringType = new Lazy<TypeDefinition>(() => GetCodeElement<TypeDefinition>(typeDefinition.GetDeclaringType()));
            _events = new Lazy<IReadOnlyList<EventDefinition>>(() => GetCodeElements<EventDefinition>(typeDefinition.GetEvents()));
            _fields = new Lazy<IReadOnlyList<FieldDefinition>>(() => GetCodeElements<FieldDefinition>(typeDefinition.GetFields()));
            _genericParameters = new Lazy<IReadOnlyList<GenericParameter>>(() => GetCodeElements<GenericParameter>(typeDefinition.GetGenericParameters()));
            _interfaceImplementations = new Lazy<IReadOnlyList<InterfaceImplementation>>(() => GetCodeElements<InterfaceImplementation>(typeDefinition.GetInterfaceImplementations()));
            Layout = typeDefinition.GetLayout();
            _nestedTypes = new Lazy<IReadOnlyList<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(typeDefinition.GetNestedTypes()));
            _properties = new Lazy<IReadOnlyList<PropertyDefinition>>(() => GetCodeElements<PropertyDefinition>(typeDefinition.GetProperties()));
        }

        public TypeAttributes Attributes { get; }

        public CodeElement BaseType => _baseType.Value;

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public IReadOnlyList<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public TypeDefinition DeclaringType => _declaringType.Value;

        public IReadOnlyList<EventDefinition> Events => _events.Value;

        public IReadOnlyList<FieldDefinition> Fields => _fields.Value;

        public IReadOnlyList<GenericParameter> GenericParameters => _genericParameters.Value;

        public IReadOnlyList<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public TypeLayout Layout { get; }

        public IReadOnlyList<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        public IReadOnlyList<MethodDefinition> Methods => _methods.Value;

        public string Name => _name.Value;

        public string Namespace => _namespace.Value;

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        public IEnumerable<TypeDefinition> NestedTypes => _nestedTypes.Value;

        public IEnumerable<PropertyDefinition> Properties => _properties.Value;
    }
}
