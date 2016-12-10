using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition" />
    [PublicAPI]
    public class TypeDefinition : TypeBase, ICodeElementWithHandle<TypeDefinitionHandle, System.Reflection.Metadata.TypeDefinition>
    {
        private readonly Lazy<TypeBase> _baseType;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IEnumerable<EventDefinition>> _events;
        private readonly Lazy<IEnumerable<FieldDefinition>> _fields;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<IEnumerable<InterfaceImplementation>> _interfaceImplementations;
        private readonly Lazy<IEnumerable<MethodImplementation>> _methodImplementations;
        private readonly Lazy<IEnumerable<MethodDefinitionBase>> _methods;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;
        private readonly Lazy<IEnumerable<TypeDefinition>> _nestedTypes;
        private readonly Lazy<IEnumerable<PropertyDefinition>> _properties;

        internal TypeDefinition(TypeDefinitionHandle typeDefinitionHandle, MetadataState metadataState) : base(typeDefinitionHandle, metadataState)
        {
            MetadataHandle = typeDefinitionHandle;
            DowncastMetadataHandle = MetadataHandle;
            MetadataToken = Reader.GetTypeDefinition(typeDefinitionHandle);
            _namespaceDefinition = GetLazyCodeElementWithHandle<NamespaceDefinition>(MetadataToken.NamespaceDefinition);
            _methods = GetLazyCodeElementsWithHandle<MethodDefinitionBase>(MetadataToken.GetMethods());
            _methodImplementations = GetLazyCodeElementsWithHandle<MethodImplementation>(MetadataToken.GetMethodImplementations());
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _baseType = new Lazy<TypeBase>(() => GetCodeElementWithHandle<TypeBase>(MetadataToken.BaseType));
            Namespace = AsString(MetadataToken.Namespace);
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declarativeSecurityAttributes = GetLazyCodeElementsWithHandle<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _declaringType = GetLazyCodeElementWithHandle<TypeDefinition>(MetadataToken.GetDeclaringType());
            _events = GetLazyCodeElementsWithHandle<EventDefinition>(MetadataToken.GetEvents());
            _fields = GetLazyCodeElementsWithHandle<FieldDefinition>(MetadataToken.GetFields());
            _genericParameters = GetLazyCodeElementsWithHandle<GenericParameter>(MetadataToken.GetGenericParameters());
            _interfaceImplementations = GetLazyCodeElementsWithHandle<InterfaceImplementation>(MetadataToken.GetInterfaceImplementations());
            Layout = MetadataToken.GetLayout();
            _nestedTypes = GetLazyCodeElementsWithHandle<TypeDefinition>(MetadataToken.GetNestedTypes());
            _properties = GetLazyCodeElementsWithHandle<PropertyDefinition>(MetadataToken.GetProperties());
        }

        public override string FullName => (IsNested && BaseType != null ? BaseType.FullName + "." : string.Empty) + (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".") + Name;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Attributes" />
        public TypeAttributes Attributes { get; }

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

        public IEnumerable<Document> Documents { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetEvents" />
        public IEnumerable<EventDefinition> Events => _events.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetFields" />
        public IEnumerable<FieldDefinition> Fields => _fields.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetGenericParameters" />
        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetInterfaceImplementations" />
        public IEnumerable<InterfaceImplementation> InterfaceImplementations => _interfaceImplementations.Value;

        public IEnumerable<Language> Languages { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetLayout" />
        public TypeLayout Layout { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethodImplementations" />
        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetMethods" />
        public IEnumerable<MethodDefinitionBase> Methods => _methods.Value;

        public override string Name { get; }

        public override string Namespace { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.NamespaceDefinition" />
        /// <summary>The definition handle of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetNestedTypes" />
        public IEnumerable<TypeDefinition> NestedTypes => _nestedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.GetProperties" />
        public IEnumerable<PropertyDefinition> Properties => _properties.Value;

        public Handle DowncastMetadataHandle { get; }

        public TypeDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.TypeDefinition MetadataToken { get; }

        #region members mirroring System.Type

        /// <inheritdoc cref="System.Type.AssemblyQualifiedName" />
        public string AssemblyQualifiedName { get; }

        /// <inheritdoc cref="System.Type.GenericParameterPosition" />
        public int GenericParameterPosition { get; }

        /// <inheritdoc cref="System.Type.HasElementType" />
        public bool HasElementType { get; }

        /// <inheritdoc cref="System.Type.IsArray" />
        public bool IsArray { get; }

        /// <inheritdoc cref="System.Type.IsByRef" />
        public bool IsByRef { get; }

        /// <inheritdoc cref="System.Type.IsConstructedGenericType" />
        public bool IsConstructedGenericType { get; }

        /// <inheritdoc cref="System.Type.IsNested" />
        public bool IsNested => IsNestedAssembly || IsNestedFamily || IsNestedFamANDAssem || IsNestedFamORAssem || IsNestedPrivate || IsNestedPublic;

        /// <inheritdoc cref="System.Type.IsPointer" />
        public bool IsPointer { get; }

        /// <inheritdoc cref="System.Type.GetArrayRank" />
        public int GetArrayRank()
        {
            throw new NotImplementedException();
        }

        private bool Is(TypeAttributes typeAttribute) => (Attributes & typeAttribute) != 0;

        /// 
        public bool IsAbstract => Is(TypeAttributes.Abstract);

        public bool IsAnsiClass => Is(TypeAttributes.AnsiClass);

        public bool IsAutoClass => Is(TypeAttributes.AutoClass);

        public bool IsAutoLayout => Is(TypeAttributes.AutoLayout);

        public bool IsClass => Is(TypeAttributes.Class);

        public virtual bool IsCOMObject { get; }

        public bool IsEnum => "System.Enum".Equals(BaseType?.FullName);

        public bool IsExplicitLayout => Is(TypeAttributes.ExplicitLayout);

        public bool IsGenericParameter { get; }

        public bool IsGenericType { get; }

        public bool IsGenericTypeDefinition { get; }

        public bool IsImport => Is(TypeAttributes.Import);

        public bool IsInterface => Is(TypeAttributes.Interface);

        public bool IsLayoutSequential => Is(TypeAttributes.SequentialLayout);

        public bool IsMarshalByRef { get; }

        public bool IsNestedAssembly => Is(TypeAttributes.NestedAssembly);

        public bool IsNestedFamANDAssem => Is(TypeAttributes.NestedFamANDAssem);

        public bool IsNestedFamily => Is(TypeAttributes.NestedFamily);

        public bool IsNestedFamORAssem => Is(TypeAttributes.NestedFamORAssem);

        public bool IsNestedPrivate => Is(TypeAttributes.NestedPrivate);

        public bool IsNestedPublic => Is(TypeAttributes.NestedPublic);

        public bool IsNotPublic => Is(TypeAttributes.NotPublic);

        public bool IsPrimitive { get; }

        public bool IsPublic => Is(TypeAttributes.Public);

        public bool IsSealed => Is(TypeAttributes.Sealed);

        public bool IsSerializable => Is(TypeAttributes.Serializable);

        public bool IsSpecialName => Is(TypeAttributes.SpecialName);

        public bool IsUnicodeClass => Is(TypeAttributes.UnicodeClass);

        public bool IsValueType { get; }

        public bool IsVisible => Is(TypeAttributes.VisibilityMask);

        /// <inheritdoc cref="System.Type.GetElementType" />
        public TypeDefinition GetElementType()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
