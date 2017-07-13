using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class TypeDefinition : TypeBase<TypeDefinition, TypeDefinitionHandle, System.Reflection.Metadata.TypeDefinition>
    {
        private Lazy<IEnumerable<MethodBase>> _allMethods;
        private Lazy<TypeBase> _baseType;
        private Lazy<IEnumerable<ConstructorDefinition>> _constructors;
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private Lazy<TypeDefinition> _declaringType;
        private Lazy<IEnumerable<EventDefinition>> _events;
        private Lazy<IEnumerable<FieldDefinition>> _fields;
        private Lazy<Type[]> _genericParameters;
        private Lazy<IEnumerable<TypeInfo>> _interfaceImplementations;
        private Lazy<IEnumerable<MemberInfo>> _members;
        private Lazy<IEnumerable<MethodImplementation>> _methodImplementations;
        private Lazy<IEnumerable<MethodDefinition>> _methods;
        private Lazy<string> _namespace;
        private Lazy<NamespaceDefinition> _namespaceDefinition;
        private Lazy<IEnumerable<TypeInfo>> _nestedTypes;
        private Lazy<IEnumerable<PropertyDefinition>> _properties;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeDefinition(TypeDefinition baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
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

        public override System.Reflection.Assembly Assembly => MetadataState.AssemblyDefinition;

        public override Type BaseType => _baseType.Value;

        public override bool ContainsGenericParameters => GenericTypeParameters.Any();

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override IEnumerable<System.Reflection.ConstructorInfo> DeclaredConstructors => _constructors.Value;

        public override IEnumerable<System.Reflection.EventInfo> DeclaredEvents => _events.Value;

        public override IEnumerable<System.Reflection.FieldInfo> DeclaredFields => _fields.Value;

        public override IEnumerable<MemberInfo> DeclaredMembers => _members.Value;

        public override IEnumerable<System.Reflection.MethodInfo> DeclaredMethods => _methods.Value;

        public override IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes => _nestedTypes.Value;

        public override IEnumerable<System.Reflection.PropertyInfo> DeclaredProperties => _properties.Value;

        public override MethodBase DeclaringMethod => throw NotSupportedHelper.FutureVersion();

        public override Type DeclaringType => _declaringType.Value;

        public override IEnumerable<Document> Documents { get; } = ImmutableArray<Document>.Empty;

        public override GenericParameterAttributes GenericParameterAttributes => throw NotSupportedHelper.FutureVersion();

        public override int GenericParameterPosition => throw NotSupportedHelper.FutureVersion();

        public override Type[] GenericTypeParameters => _genericParameters.Value;

        public override IEnumerable<Type> ImplementedInterfaces => _interfaceImplementations.Value;

        public override bool IsConstructedGenericType => GenericTypeDefinition != null;

        public override bool IsDelegate => "System".Equals(BaseType?.Namespace) && ("Delegate".Equals(BaseType?.Name) || "MulticastDelegate".Equals(BaseType?.Name));

        public override bool IsEnum => "System.Enum".Equals(BaseType?.FullName);

        public override bool IsGenericParameter => false;

        public override bool IsGenericType => base.IsGenericType || GenericTypeParameters.Any();

        public override bool IsGenericTypeDefinition => GenericTypeParameters.Any();

        public override bool IsSerializable => (Attributes & TypeAttributes.Serializable) != 0 || IsEnum || IsDelegate;

        public override Language? Language => MetadataState.Language;

        public TypeLayout Layout => RawMetadata.GetLayout();

        public override MemberTypes MemberType => DeclaringType == null ? MemberTypes.TypeInfo : MemberTypes.NestedType;

        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Namespace => _namespace.Value;

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => throw NotSupportedHelper.FutureVersion();

        public override string SourceCode => throw NotSupportedHelper.FutureVersion();

        public override StructLayoutAttribute StructLayoutAttribute => throw NotSupportedHelper.FutureVersion();

        internal override string MetadataNamespace => RawMetadata.Namespace.IsNil ? null : MetadataState.AssemblyReader.GetString(RawMetadata.Namespace);

        internal override string UndecoratedName => MetadataState.AssemblyReader.GetString(RawMetadata.Name);

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetGenericArguments() => IsDelegate ? _genericParameters.Value : base.GetGenericArguments();

        protected override TypeAttributes GetAttributeFlagsImpl() => RawMetadata.Attributes;

        protected override bool IsMarshalByRefImpl() => throw NotSupportedHelper.FutureVersion();

        private void Initialize()
        {
            _namespaceDefinition = new Lazy<NamespaceDefinition>(() =>
            {
                NamespaceDefinition namespaceDefinition;
                /*
                 * Some assemblies use the namespace string reference also for the namespace definition.  I'm not sure if this is legal IL, but it causes an exception when
                 * you try to get the namespace definition. -- Jonathan Byrne 02/01/2017
                 */
                if (RawMetadata.NamespaceDefinition.GetHashCode() == RawMetadata.Namespace.GetHashCode())
                {
                    namespaceDefinition = null;
                }
                else
                {
                    namespaceDefinition = MetadataState.GetCodeElement<NamespaceDefinition>(RawMetadata.NamespaceDefinition);
                }
                return namespaceDefinition;
            });
            _namespace = new Lazy<string>(() =>
            {
                string @namespace;
                if (DeclaringType != null)
                {
                    @namespace = DeclaringType.Namespace;
                }
                else if (!RawMetadata.Namespace.IsNil)
                {
                    @namespace = MetadataState.AssemblyReader.GetString(RawMetadata.Namespace);
                }
                else if (NamespaceDefinition != null)
                {
                    @namespace = NamespaceDefinition.Name;
                }
                else
                {
                    @namespace = null;
                }

                return @namespace;
            });
            _allMethods = MetadataState.GetLazyCodeElements<MethodBase>(RawMetadata.GetMethods());
            _methods = new Lazy<IEnumerable<MethodDefinition>>(() => _allMethods.Value.OfType<MethodDefinition>().ToImmutableArray());
            _constructors = new Lazy<IEnumerable<ConstructorDefinition>>(() => _allMethods.Value.OfType<ConstructorDefinition>().ToImmutableArray());
            _methodImplementations = MetadataState.GetLazyCodeElements<MethodImplementation>(RawMetadata.GetMethodImplementations());
            _baseType = new Lazy<TypeBase>(() =>
            {
                TypeBase baseType;
                if (RawMetadata.BaseType.Kind == HandleKind.TypeSpecification)
                {
                    baseType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.BaseType, this);
                }
                else
                {
                    baseType = (TypeBase) MetadataState.GetCodeElement(RawMetadata.BaseType);
                }
                return baseType;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _events = MetadataState.GetLazyCodeElements<EventDefinition>(RawMetadata.GetEvents());
            _fields = MetadataState.GetLazyCodeElements<FieldDefinition>(RawMetadata.GetFields());
            _genericParameters = new Lazy<Type[]>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.SetDeclaringType(this);
                }

                return genericParameters.Cast<Type>().ToArray();
            });
            _interfaceImplementations = new Lazy<IEnumerable<TypeInfo>>(() => RawMetadata.GetInterfaceImplementations().Select(interfaceImplementationMetadata => (TypeInfo) MetadataState.GetCodeElement<InterfaceImplementation>(interfaceImplementationMetadata, this).Interface).ToImmutableArray());
            _nestedTypes = MetadataState.GetLazyCodeElements<TypeDefinition, TypeInfo>(RawMetadata.GetNestedTypes());
            _properties = MetadataState.GetLazyCodeElements<PropertyDefinition>(RawMetadata.GetProperties());
            _members = new Lazy<IEnumerable<MemberInfo>>(() => DeclaredMethods.Union<MemberInfo>(DeclaredFields).Union(DeclaredConstructors).Union(DeclaredEvents).Union(DeclaredProperties).Union(DeclaredNestedTypes).ToImmutableArray());
        }
    }
}
