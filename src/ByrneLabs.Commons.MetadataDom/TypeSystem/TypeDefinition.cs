using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using MethodBaseToExpose = System.Reflection.MethodBase;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using ModuleToExpose = System.Reflection.Module;
using AssemblyToExpose = System.Reflection.Assembly;
using EventInfoToExpose = System.Reflection.EventInfo;
using FieldInfoToExpose = System.Reflection.FieldInfo;
using MemberInfoToExpose = System.Reflection.MemberInfo;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;
using MemberInfoToExpose = ByrneLabs.Commons.MetadataDom.MemberInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class TypeDefinition : TypeBase<TypeDefinition, TypeDefinitionHandle, System.Reflection.Metadata.TypeDefinition>
    {
        private Lazy<ImmutableArray<MethodBaseToExpose>> _allMethods;
        private Lazy<TypeBase> _baseType;
        private Lazy<ImmutableArray<ConstructorInfo>> _constructors;
        private Lazy<ImmutableArray<CustomAttributeData>> _customAttributes;
        private Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private Lazy<TypeDefinition> _declaringType;
        private Lazy<ImmutableArray<EventInfo>> _events;
        private Lazy<ImmutableArray<FieldInfo>> _fields;
        private Lazy<Type[]> _genericParameters;
        private Lazy<ImmutableArray<TypeInfo>> _interfaceImplementations;
        private Lazy<ImmutableArray<MemberInfoToExpose>> _members;
        private Lazy<ImmutableArray<MethodImplementation>> _methodImplementations;
        private Lazy<ImmutableArray<MethodInfo>> _methods;
        private Lazy<string> _namespace;
        private Lazy<NamespaceDefinition> _namespaceDefinition;
        private Lazy<ImmutableArray<TypeInfo>> _nestedTypes;
        private Lazy<ImmutableArray<PropertyInfo>> _properties;

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

        public override AssemblyToExpose Assembly => MetadataState.AssemblyDefinition;

        public override TypeToExpose BaseType => _baseType.Value;

        public override bool ContainsGenericParameters => GenericTypeParameters.Any();

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override IEnumerable<ConstructorInfoToExpose> DeclaredConstructors => _constructors.Value;

        public override IEnumerable<EventInfoToExpose> DeclaredEvents => _events.Value;

        public override IEnumerable<FieldInfoToExpose> DeclaredFields => _fields.Value;

        public override IEnumerable<MemberInfoToExpose> DeclaredMembers => _members.Value;

        public override IEnumerable<MethodInfoToExpose> DeclaredMethods => _methods.Value;

        public override IEnumerable<TypeInfoToExpose> DeclaredNestedTypes => _nestedTypes.Value;

        public override IEnumerable<PropertyInfoToExpose> DeclaredProperties => _properties.Value;

        public override MethodBaseToExpose DeclaringMethod => throw NotSupportedHelper.FutureVersion();

        public override TypeToExpose DeclaringType => _declaringType.Value;

        public override IEnumerable<Document> Documents { get; } = ImmutableArray<Document>.Empty;

        public override GenericParameterAttributes GenericParameterAttributes => throw NotSupportedHelper.FutureVersion();

        public override int GenericParameterPosition => throw NotSupportedHelper.FutureVersion();

        public override TypeToExpose[] GenericTypeParameters => _genericParameters.Value;

        public override IEnumerable<TypeToExpose> ImplementedInterfaces => _interfaceImplementations.Value;

        public override bool IsConstructedGenericType => throw NotSupportedHelper.FutureVersion();

        public override bool IsDelegate => "System.Delegate".Equals(BaseType?.FullName) || "System.MulticastDelegate".Equals(BaseType?.FullName);

        public override bool IsEnum => "System.Enum".Equals(BaseType?.FullName);

        public override bool IsGenericParameter => false;

        public override bool IsGenericType => base.IsGenericType || GenericTypeParameters.Any();

        public override bool IsGenericTypeDefinition => GenericTypeParameters.Any();

        public override bool IsSecurityCritical => false;

        public override bool IsSecuritySafeCritical => false;

        public override bool IsSecurityTransparent => false;

        public override bool IsSerializable => (Attributes & TypeAttributes.Serializable) != 0 || IsEnum || IsDelegate;

        public override IEnumerable<Language> Languages { get; } = ImmutableArray<Language>.Empty;

        public TypeLayout Layout => RawMetadata.GetLayout();

        public override MemberTypes MemberType => DeclaringType == null ? MemberTypes.TypeInfo : MemberTypes.NestedType;

        public IEnumerable<MethodImplementation> MethodImplementations => _methodImplementations.Value;

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Namespace => _namespace.Value;

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        public override TypeToExpose ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override StructLayoutAttribute StructLayoutAttribute => throw NotSupportedHelper.FutureVersion();

        public override ConstructorInfoToExpose TypeInitializer => DeclaredConstructors.SingleOrDefault(constructor => !constructor.GetParameters().Any());

        internal override string MetadataNamespace => RawMetadata.Namespace.IsNil ? null : MetadataState.AssemblyReader.GetString(RawMetadata.Namespace);

        internal override string UndecoratedName => MetadataState.AssemblyReader.GetString(RawMetadata.Name);

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
            _methods = new Lazy<ImmutableArray<MethodInfo>>(() => _allMethods.Value.OfType<MethodInfo>().ToImmutableArray());
            _constructors = new Lazy<ImmutableArray<ConstructorInfo>>(() => _allMethods.Value.OfType<ConstructorInfo>().ToImmutableArray());
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
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _events = MetadataState.GetLazyCodeElements<EventDefinition, EventInfo>(RawMetadata.GetEvents());
            _fields = MetadataState.GetLazyCodeElements<FieldDefinition, FieldInfo>(RawMetadata.GetFields());
            _genericParameters = new Lazy<Type[]>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.SetDeclaringType(this);
                }

                return genericParameters.Cast<Type>().ToArray();
            });
            _interfaceImplementations = new Lazy<ImmutableArray<TypeInfo>>(() => RawMetadata.GetInterfaceImplementations().Select(interfaceImplementationMetadata => (TypeInfo) MetadataState.GetCodeElement<InterfaceImplementation>(interfaceImplementationMetadata, this).Interface).ToImmutableArray());
            _nestedTypes = MetadataState.GetLazyCodeElements<TypeDefinition, TypeInfo>(RawMetadata.GetNestedTypes());
            _properties = MetadataState.GetLazyCodeElements<PropertyDefinition, PropertyInfo>(RawMetadata.GetProperties());
            _members = new Lazy<ImmutableArray<MemberInfo>>(() => DeclaredMethods.Union<MemberInfo>(DeclaredFields).Union(DeclaredEvents).Union(DeclaredProperties).Union(DeclaredNestedTypes).ToImmutableArray());
        }

#if NETSTANDARD2_0 || NET_FRAMEWORK

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

#endif
    }
}
