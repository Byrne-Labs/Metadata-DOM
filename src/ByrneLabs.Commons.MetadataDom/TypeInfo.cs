using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Globalization;
using System;
using System.Linq;
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
using System.Linq;
using System;
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

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class TypeInfo
    {
        public abstract int ArrayRank { get; }

        public abstract IEnumerable<TypeSystem.Document> Documents { get; }

        public abstract TypeInfoToExpose ElementType { get; }

        public abstract TypeInfoToExpose GenericTypeDefinition { get; }

        public abstract bool IsBoxed { get; }

        public abstract bool IsByValue { get; }

        public abstract bool IsConstant { get; }

        public abstract bool IsDelegate { get; }

        public abstract bool IsVolatile { get; }

        public abstract IEnumerable<Language> Languages { get; }

        public abstract ConstructorInfoToExpose TypeInitializer { get; }

        public virtual bool HasGenericTypeArguments => GetGenericArguments().Any();

        public bool IsPrimitive => IsPrimitiveImpl();

        public bool IsStatic => IsAbstract && IsSealed;

        public override MemberTypes MemberType => IsNested ? MemberTypes.NestedType : MemberTypes.TypeInfo;

        public override TypeToExpose ReflectedType => DeclaringType;

        internal abstract string UndecoratedName { get; }

        internal static BindingFlags CalculateBindingFlags(bool isPublic, bool isInherited, bool isStatic)
        {
            var bindingFlags = isPublic ? BindingFlags.Public : BindingFlags.NonPublic;

            if (isInherited && isStatic)
            {
                bindingFlags |= BindingFlags.FlattenHierarchy;
            }
            if (isInherited)
            {
                bindingFlags |= BindingFlags.DeclaredOnly;
            }
            if (isStatic)
            {
                bindingFlags |= BindingFlags.Static;
            }
            else
            {
                bindingFlags |= BindingFlags.Instance;
            }

            return bindingFlags;
        }

        public override string ToString() => $"({GetType().FullName}) {FullName}";

        protected override bool IsPrimitiveImpl() => throw new NotImplementedException();
    }
#if NETSTANDARD2_0 || NET_FRAMEWORK

    public abstract partial class TypeInfo : TypeDelegator, IMemberInfo
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public abstract string TextSignature { get; }

        public BindingFlags BindingFlags => CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public override TypeToExpose[] GenericTypeArguments => GetGenericArguments();

        public override Guid GUID => throw NotSupportedHelper.FutureVersion();

        public bool IsCOMObject => throw NotSupportedHelper.NotValidForMetadata();

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public override bool IsConstructedGenericType => throw NotSupportedHelper.FutureVersion();

        public bool IsInherited => DeclaringType == ReflectedType;

        public override bool IsSecurityCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecuritySafeCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecurityTransparent => throw NotSupportedHelper.NotValidForMetadata();

        public override RuntimeTypeHandle TypeHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override Type UnderlyingSystemType => this;

        public override ConstructorInfoToExpose[] GetConstructors(BindingFlags bindingFlags) => GetMethods<ConstructorInfoToExpose>(null, bindingFlags, CallingConventions.Any, null, false).ToArray();

        public override EventInfoToExpose GetEvent(string name, BindingFlags bindingFlags) => SingleMember(GetEvents(name, bindingFlags, false));

        public override EventInfoToExpose[] GetEvents(BindingFlags bindingFlags) => GetEvents(null, bindingFlags, false).ToArray();

        public override EventInfoToExpose[] GetEvents() => GetEvents(DefaultBindingFlags);

        public override FieldInfoToExpose GetField(string name, BindingFlags bindingFlags) => SingleMember(GetFields(name, bindingFlags, false));

        public override FieldInfoToExpose[] GetFields(BindingFlags bindingFlags) => GetFields(null, bindingFlags, false).ToArray();

        public override int GetHashCode() => FullName.GetHashCode() | 12345;

        public override TypeToExpose GetInterface(string name, bool ignoreCase) => SingleMember(GetInterfaces(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase, false));

        public override TypeToExpose[] GetInterfaces() => GetInterfaces(null, BindingFlags.Public | BindingFlags.NonPublic, false).ToArray();

        public override MemberInfoToExpose[] GetMember(string name, MemberTypes type, BindingFlags bindingFlags)
        {
            var members = new List<MemberInfo>();
            if ((type & MemberTypes.Method) != 0)
            {
                members.AddRange(GetMethods<MethodInfo>(name, bindingFlags, CallingConventions.Any, null, true));
            }
            if ((type & MemberTypes.Constructor) != 0)
            {
                members.AddRange(GetMethods<ConstructorInfo>(name, bindingFlags, CallingConventions.Any, null, true));
            }
            if ((type & MemberTypes.Property) != 0)
            {
                members.AddRange(GetProperties(name, bindingFlags, null, null, true));
            }
            if ((type & MemberTypes.Event) != 0)
            {
                members.AddRange(GetEvents(name, bindingFlags, true));
            }
            if ((type & MemberTypes.Field) != 0)
            {
                members.AddRange(GetFields(name, bindingFlags, true));
            }
            if ((type & (MemberTypes.NestedType | MemberTypes.TypeInfo)) != 0)
            {
                members.AddRange(GetNestedTypes(name, bindingFlags, true));
            }

            return members.ToArray();
        }

        public override MemberInfoToExpose[] GetMembers(BindingFlags bindingFlags) => GetMember(null, MemberTypes.All, bindingFlags);

        public override MethodInfoToExpose[] GetMethods(BindingFlags bindingFlags) => GetMethods<MethodInfoToExpose>(null, bindingFlags, CallingConventions.Any, null, false).ToArray();

        public override TypeToExpose GetNestedType(string name, BindingFlags bindingFlags) => SingleMember(GetNestedTypes(name, bindingFlags, false));

        public override TypeToExpose[] GetNestedTypes(BindingFlags bindingFlags) => GetNestedTypes(null, bindingFlags, false).ToArray();

        public override PropertyInfoToExpose[] GetProperties(BindingFlags bindingFlags) => GetProperties(null, bindingFlags, null, null, false).ToArray();

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => throw NotSupportedHelper.NotValidForMetadata();

        protected override ConstructorInfoToExpose GetConstructorImpl(BindingFlags bindingFlags, Binder binder, CallingConventions callConvention, Type[] argumentTypes, ParameterModifier[] modifiers) => SingleMember(GetMethods<ConstructorInfoToExpose>(null, bindingFlags, callConvention, argumentTypes, false));

        protected override MethodInfoToExpose GetMethodImpl(string name, BindingFlags bindingFlags, Binder binder, CallingConventions callConvention, Type[] argumentTypes, ParameterModifier[] modifiers) => SingleMember(GetMethods<MethodInfoToExpose>(name, bindingFlags, callConvention, argumentTypes, false));

        protected override PropertyInfoToExpose GetPropertyImpl(string name, BindingFlags bindingFlags, Binder binder, Type propertyType, Type[] types, ParameterModifier[] modifiers) => SingleMember(GetProperties(name, bindingFlags, propertyType, types, false));

        protected override bool IsCOMObjectImpl() => throw NotSupportedHelper.NotValidForMetadata();

        protected override bool IsContextfulImpl() => throw NotSupportedHelper.NotValidForMetadata();

        private IEnumerable<EventInfoToExpose> GetEvents(string name, BindingFlags bindingFlags, bool allowPrefixLookup)
        {
            bindingFlags ^= BindingFlags.DeclaredOnly;

            var candidateEvents = GetMemberCandidates<EventInfo>(name, bindingFlags, false);

            // filter by binding flag
            candidateEvents = candidateEvents.Where(eventInfo => (bindingFlags & eventInfo.BindingFlags) == eventInfo.BindingFlags);

            return candidateEvents;
        }

        private IEnumerable<FieldInfoToExpose> GetFields(string name, BindingFlags bindingFlags, bool allowPrefixLookup)
        {
            bindingFlags ^= BindingFlags.DeclaredOnly;

            var candidateFields = GetMemberCandidates<FieldInfo>(name, bindingFlags, false);

            // filter by binding flag
            candidateFields = candidateFields.Where(field => (bindingFlags & field.BindingFlags) == field.BindingFlags);

            return candidateFields;
        }

        private IEnumerable<TypeToExpose> GetInterfaces(string name, BindingFlags bindingFlags, bool allowPrefixLookup) => GetTypes(name, bindingFlags, allowPrefixLookup).Where(type => type.IsInterface);

        private IEnumerable<T> GetMemberCandidates<T>(string name, BindingFlags bindingFlags, bool allowPrefixLookup) where T : MemberInfoToExpose
        {
            // filter by type
            var candidateMembers = DeclaredMembers.OfType<T>();

            // filter by name
            if (name != null)
            {
                var stringComparison = (bindingFlags & BindingFlags.IgnoreCase) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (allowPrefixLookup && name.EndsWith("*", StringComparison.Ordinal))
                {
                    var prefix = name.Substring(0, name.Length - 1);
                    candidateMembers = candidateMembers.Where(member => member.Name.StartsWith(prefix, stringComparison));
                }
                else
                {
                    candidateMembers = candidateMembers.Where(member => member.Name.Equals(name, stringComparison));
                }
            }

            return candidateMembers.ToImmutableArray();
        }

        private IEnumerable<T> GetMethods<T>(string name, BindingFlags bindingFlags, CallingConventions callConvention, Type[] argumentTypes, bool allowPrefixLookup) where T : MethodBaseToExpose
        {
            var candidateMethods = GetMemberCandidates<T>(name, bindingFlags, false);

            // filter by calling convention
            if ((callConvention & CallingConventions.Any) == 0)
            {
                candidateMethods = candidateMethods.Where(methodBase =>
                    ((callConvention & CallingConventions.VarArgs) == 0 || (methodBase.CallingConvention & CallingConventions.VarArgs) == 0) &&
                    ((callConvention & CallingConventions.Standard) == 0 || (methodBase.CallingConvention & CallingConventions.Standard) == 0));
            }

            // filter by arguments
            if (argumentTypes != null && argumentTypes.Length > 0)
            {
                if ((bindingFlags & (BindingFlags.GetProperty | BindingFlags.SetProperty)) == 0)
                {
                    candidateMethods = candidateMethods.Where(method => method.GetParameters().Length == argumentTypes.Length);
                }
                candidateMethods = candidateMethods.Where(method =>
                {
                    var includeMethod = true;
                    var parameters = method.GetParameters();
                    var checkForVarArgs = false;

                    if (argumentTypes.Length > parameters.Length)
                    {
                        if ((method.CallingConvention & CallingConventions.VarArgs) == 0)
                        {
                            checkForVarArgs = true;
                        }
                    }
                    else if ((bindingFlags & BindingFlags.OptionalParamBinding) == 0 || !parameters[argumentTypes.Length].IsOptional)
                    {
                        checkForVarArgs = true;
                    }

                    if (checkForVarArgs)
                    {
                        if (parameters.Length == 0 ||
                            argumentTypes.Length < parameters.Length - 1 ||
                            !parameters.Last().ParameterType.IsArray ||
                            !parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                        {
                            includeMethod = false;
                        }
                    }

                    return includeMethod;
                });
            }

            return candidateMethods;
        }

        private IEnumerable<TypeToExpose> GetNestedTypes(string name, BindingFlags bindingFlags, bool allowPrefixLookup) => GetTypes(name, bindingFlags, allowPrefixLookup).Where(type => type.IsNested);

        private IEnumerable<PropertyInfoToExpose> GetProperties(string name, BindingFlags bindingFlags, Type propertyType, Type[] types, bool allowPrefixLookup)
        {
            bindingFlags ^= BindingFlags.DeclaredOnly;

            var candidateProperties = GetMemberCandidates<PropertyInfo>(name, bindingFlags, false);

            // filter by binding flag
            candidateProperties = candidateProperties.Where(property => (bindingFlags & property.BindingFlags) == property.BindingFlags);

            // filter by property type
            if (propertyType != null)
            {
                candidateProperties.Where(property => property.PropertyType.IsEquivalentTo(propertyType));
            }

            // filter by parameter types
            if (types != null && types.Length > 0)
            {
                candidateProperties = candidateProperties.Where(property => property.GetIndexParameters().Select(parameter => parameter.ParameterType).SequenceEqual(types));
            }

            return candidateProperties;
        }

        private IEnumerable<TypeToExpose> GetTypes(string fullName, BindingFlags bindingFlags, bool allowPrefixLookup)
        {
            bindingFlags &= ~BindingFlags.Static;

            var candidateTypes = GetMemberCandidates<TypeInfo>(null, bindingFlags, false);

            // filter on binding flags
            if ((bindingFlags & BindingFlags.Public) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => nestedClass.IsNestedPublic || nestedClass.IsPublic);
            }
            if ((bindingFlags & BindingFlags.NonPublic) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => !nestedClass.IsNestedPublic && !nestedClass.IsPublic);
            }
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => nestedClass.IsInherited);
            }
            if ((bindingFlags & BindingFlags.FlattenHierarchy) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => !nestedClass.IsStatic || nestedClass.IsInherited);
            }
            if ((bindingFlags & BindingFlags.Static) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => nestedClass.IsStatic);
            }
            if ((bindingFlags & BindingFlags.Instance) != 0)
            {
                candidateTypes = candidateTypes.Where(nestedClass => !nestedClass.IsStatic);
            }

            // filter on name
            if (fullName != null)
            {
                string @namespace;
                string name;
                var lastNamespaceSeperator = fullName.LastIndexOf(".", StringComparison.Ordinal);
                if (lastNamespaceSeperator == -1)
                {
                    @namespace = null;
                    name = fullName;
                }
                else if (lastNamespaceSeperator > fullName.Length || lastNamespaceSeperator == 0)
                {
                    throw new ArgumentException("Name cannot begin or end with a dot", nameof(fullName));
                }
                else
                {
                    @namespace = fullName.Substring(0, lastNamespaceSeperator - 1);
                    name = fullName.Substring(lastNamespaceSeperator);
                }

                if (@namespace != null)
                {
                    candidateTypes = candidateTypes.Where(type => type.Namespace.Equals(@namespace));
                }

                var stringComparison = (bindingFlags & BindingFlags.IgnoreCase) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (allowPrefixLookup && name.EndsWith("*", StringComparison.Ordinal))
                {
                    var prefix = name.Substring(0, name.Length - 1);
                    candidateTypes = candidateTypes.Where(type => type.Name.StartsWith(prefix, stringComparison));
                }
                else
                {
                    candidateTypes = candidateTypes.Where(type => type.Name.Equals(name, stringComparison));
                }
            }

            return candidateTypes;
        }

        private T SingleMember<T>(IEnumerable<T> members) where T : MemberInfoToExpose
        {
            if (members.Count() > 1)
            {
                throw new AmbiguousMatchException($"Multiple {typeof(T).Name.ToLower()} instances match arguments provided");
            }

            return members.FirstOrDefault();
        }
    }
#else
    public abstract partial class TypeInfo : Type
    {

        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();

        public abstract TypeToExpose[] GetGenericArguments();

        public abstract TypeToExpose GetGenericTypeDefinition();

        public abstract bool ContainsGenericParameters { get; }

        public abstract MethodBaseToExpose DeclaringMethod { get; }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public abstract TypeToExpose[] GenericTypeParameters { get; }

        public abstract bool IsGenericTypeDefinition { get; }

        public abstract bool IsSecurityCritical { get; }

        public abstract bool IsSecuritySafeCritical { get; }

        public abstract bool IsSecurityTransparent { get; }

        public abstract StructLayoutAttribute StructLayoutAttribute { get; }

        public virtual IEnumerable<ConstructorInfoToExpose> DeclaredConstructors => GetConstructors(DeclaredOnlyLookup);

        public virtual IEnumerable<EventInfoToExpose> DeclaredEvents => GetEvents(DeclaredOnlyLookup);

        public virtual IEnumerable<FieldInfoToExpose> DeclaredFields => GetFields(DeclaredOnlyLookup);

        public virtual IEnumerable<MemberInfoToExpose> DeclaredMembers => GetMembers(DeclaredOnlyLookup);

        public virtual IEnumerable<MethodInfoToExpose> DeclaredMethods => GetMethods(DeclaredOnlyLookup);

        public virtual IEnumerable<TypeInfoToExpose> DeclaredNestedTypes => GetNestedTypes(DeclaredOnlyLookup).Cast<TypeInfoToExpose>();

        public virtual IEnumerable<PropertyInfoToExpose> DeclaredProperties => GetProperties(DeclaredOnlyLookup);

        public virtual GenericParameterAttributes GenericParameterAttributes => throw NotSupportedHelper.FutureVersion();

        public virtual IEnumerable<TypeToExpose> ImplementedInterfaces => GetInterfaces();
    }
#endif
}
