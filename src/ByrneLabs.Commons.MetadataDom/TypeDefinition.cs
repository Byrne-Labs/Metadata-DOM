using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("{Namespace}.{Name}")]
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

        public TypeAttributes Attributes { get; }

        public CodeElement BaseType => _baseType.Value;

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public TypeDefinition DeclaringType => _declaringType.Value;

        public IEnumerable<EventDefinition> Events => _events.Value;

        public IEnumerable<FieldDefinition> Fields => _fields.Value;

        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        public IEnumerable<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public TypeLayout Layout { get; }

        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        public IEnumerable<MethodDefinitionBase> Methods => _methods.Value;

        public string Name => _name.Value;

        public string Namespace => _namespace.Value;

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        public IEnumerable<TypeDefinition> NestedTypes => _nestedTypes.Value;

        public IEnumerable<PropertyDefinition> Properties => _properties.Value;
    }
}
