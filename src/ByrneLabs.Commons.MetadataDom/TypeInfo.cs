using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class TypeInfo : TypeDelegator, IMemberInfo
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public abstract int ArrayRank { get; }

        public abstract IEnumerable<TypeSystem.Document> Documents { get; }

        public abstract TypeInfo ElementType { get; }

        public abstract string FullTextSignature { get; }

        public abstract TypeInfo GenericTypeDefinition { get; }

        public abstract bool IsBoxed { get; }

        public abstract bool IsByValue { get; }

        public abstract bool IsConstant { get; }

        public abstract bool IsDelegate { get; }

        public abstract bool IsVolatile { get; }

        public abstract Language? Language { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public BindingFlags BindingFlags => CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public override Type[] GenericTypeArguments => GetGenericArguments();

        public override Guid GUID => throw NotSupportedHelper.FutureVersion();

        public virtual bool HasGenericTypeArguments => GetGenericArguments().Any();

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public override bool IsConstructedGenericType => throw NotSupportedHelper.FutureVersion();

        public bool IsInherited => DeclaringType == ReflectedType;

        public override bool IsSecurityCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecuritySafeCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecurityTransparent => throw NotSupportedHelper.NotValidForMetadata();

        public bool IsStatic => IsAbstract && IsSealed;

        public override MemberTypes MemberType => IsNested ? MemberTypes.NestedType : MemberTypes.TypeInfo;

        public override Type ReflectedType => DeclaringType;

        public virtual string TextSignature => Name;

        public override RuntimeTypeHandle TypeHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override Type UnderlyingSystemType => this;

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

        public override System.Reflection.ConstructorInfo[] GetConstructors(BindingFlags bindingFlags) => GetMethods<System.Reflection.ConstructorInfo>(null, bindingFlags, CallingConventions.Any, null, false).ToArray();

        public override System.Reflection.EventInfo GetEvent(string name, BindingFlags bindingFlags) => SingleMember(GetEvents(name, bindingFlags, false));

        public override System.Reflection.EventInfo[] GetEvents(BindingFlags bindingFlags) => GetEvents(null, bindingFlags, false).ToArray();

        public override System.Reflection.EventInfo[] GetEvents() => GetEvents(DefaultBindingFlags);

        public override System.Reflection.FieldInfo GetField(string name, BindingFlags bindingFlags) => SingleMember(GetFields(name, bindingFlags, false));

        public override System.Reflection.FieldInfo[] GetFields(BindingFlags bindingFlags) => GetFields(null, bindingFlags, false).ToArray();

        public override Type GetInterface(string name, bool ignoreCase) => SingleMember(GetInterfaces(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase, false));

        public override Type[] GetInterfaces() => GetInterfaces(null, BindingFlags.Public | BindingFlags.NonPublic, false).ToArray();

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingFlags)
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

        public override MemberInfo[] GetMembers(BindingFlags bindingFlags) => GetMember(null, MemberTypes.All, bindingFlags);

        public override System.Reflection.MethodInfo[] GetMethods(BindingFlags bindingFlags) => GetMethods<System.Reflection.MethodInfo>(null, bindingFlags, CallingConventions.Any, null, false).ToArray();

        public override Type GetNestedType(string name, BindingFlags bindingFlags) => SingleMember(GetNestedTypes(name, bindingFlags, false));

        public override Type[] GetNestedTypes(BindingFlags bindingFlags) => GetNestedTypes(null, bindingFlags, false).ToArray();

        public override System.Reflection.PropertyInfo[] GetProperties(BindingFlags bindingFlags) => GetProperties(null, bindingFlags, null, null, false).ToArray();

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => throw NotSupportedHelper.NotValidForMetadata();

        public override string ToString() => FullName;

        protected override System.Reflection.ConstructorInfo GetConstructorImpl(BindingFlags bindingFlags, Binder binder, CallingConventions callConvention, Type[] argumentTypes, ParameterModifier[] modifiers) => SingleMember(GetMethods<ConstructorInfo>(null, bindingFlags, callConvention, argumentTypes, false));

        protected override System.Reflection.MethodInfo GetMethodImpl(string name, BindingFlags bindingFlags, Binder binder, CallingConventions callConvention, Type[] argumentTypes, ParameterModifier[] modifiers) => SingleMember(GetMethods<MethodInfo>(name, bindingFlags, callConvention, argumentTypes, false));

        protected override System.Reflection.PropertyInfo GetPropertyImpl(string name, BindingFlags bindingFlags, Binder binder, Type propertyType, Type[] types, ParameterModifier[] modifiers) => SingleMember(GetProperties(name, bindingFlags, propertyType, types, false));

        protected override bool IsCOMObjectImpl() => throw NotSupportedHelper.NotValidForMetadata();

        protected override bool IsContextfulImpl() => throw NotSupportedHelper.NotValidForMetadata();

        protected override bool IsPrimitiveImpl() => throw NotSupportedHelper.FutureVersion();

        private IEnumerable<System.Reflection.EventInfo> GetEvents(string name, BindingFlags bindingFlags, bool allowPrefixLookup)
        {
            bindingFlags ^= BindingFlags.DeclaredOnly;

            var candidateEvents = GetMemberCandidates<EventInfo>(name, bindingFlags, false);

            // filter by binding flag
            candidateEvents = candidateEvents.Where(eventInfo => (bindingFlags & eventInfo.BindingFlags) == eventInfo.BindingFlags);

            return candidateEvents;
        }

        private IEnumerable<System.Reflection.FieldInfo> GetFields(string name, BindingFlags bindingFlags, bool allowPrefixLookup)
        {
            bindingFlags ^= BindingFlags.DeclaredOnly;

            var candidateFields = GetMemberCandidates<FieldInfo>(name, bindingFlags, false);

            // filter by binding flag
            candidateFields = candidateFields.Where(field => (bindingFlags & field.BindingFlags) == field.BindingFlags);

            return candidateFields;
        }

        private IEnumerable<Type> GetInterfaces(string name, BindingFlags bindingFlags, bool allowPrefixLookup) => GetTypes(name, bindingFlags, allowPrefixLookup).Where(type => type.IsInterface);

        private IEnumerable<T> GetMemberCandidates<T>(string name, BindingFlags bindingFlags, bool allowPrefixLookup) where T : MemberInfo
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

        private IEnumerable<T> GetMethods<T>(string name, BindingFlags bindingFlags, CallingConventions callConvention, Type[] argumentTypes, bool allowPrefixLookup) where T : MethodBase
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
            if (argumentTypes != null)
            {
                if ((bindingFlags & (BindingFlags.GetProperty | BindingFlags.SetProperty)) == 0)
                {
                    candidateMethods = candidateMethods.Where(method => method.GetParameters().Length == argumentTypes.Length);
                }
                if (argumentTypes.Length > 0)
                {
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
            }

            return candidateMethods;
        }

        private IEnumerable<Type> GetNestedTypes(string name, BindingFlags bindingFlags, bool allowPrefixLookup) => GetTypes(name, bindingFlags, allowPrefixLookup).Where(type => type.IsNested);

        private IEnumerable<System.Reflection.PropertyInfo> GetProperties(string name, BindingFlags bindingFlags, Type propertyType, Type[] types, bool allowPrefixLookup)
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

        private IEnumerable<Type> GetTypes(string fullName, BindingFlags bindingFlags, bool allowPrefixLookup)
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

        private T SingleMember<T>(IEnumerable<T> members) where T : MemberInfo
        {
            if (members.Count() > 1)
            {
                throw new AmbiguousMatchException($"Multiple {typeof(T).Name.ToLowerInvariant()} instances match arguments provided");
            }

            return members.SingleOrDefault();
        }
    }
}
