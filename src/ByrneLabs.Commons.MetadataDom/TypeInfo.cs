using System.Collections.Generic;
using System.Reflection;
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
    public abstract partial class TypeInfo
    {
        public abstract int ArrayRank { get; }

        public abstract IEnumerable<TypeSystem.Document> Documents { get; }

        public abstract TypeInfoToExpose ElementType { get; }

        public abstract TypeInfoToExpose GenericTypeDefinition { get; }

        public abstract bool HasGenericTypeArguments { get; }

        public abstract bool IsBoxed { get; }

        public abstract bool IsByValue { get; }

        public abstract bool IsConstant { get; }

        public abstract bool IsDelegate { get; }

        public abstract bool IsVolatile { get; }

        public abstract IEnumerable<Language> Languages { get; }

        public abstract ConstructorInfoToExpose TypeInitializer { get; }

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
    }
#if NETSTANDARD2_0 || NET_FRAMEWORK

    public abstract partial class TypeInfo : TypeDelegator, IMemberInfo
    {
        public abstract string TextSignature { get; }

        public override TypeToExpose[] GenericTypeArguments => GetGenericArguments();

        public override Guid GUID => throw new NotSupportedException();

        public bool IsCOMObject => throw new NotSupportedException();

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public override bool IsConstructedGenericType => throw new NotSupportedException();

        public override EventInfoToExpose GetEvent(string name, BindingFlags bindingAttr) => throw new NotSupportedException();

        public override EventInfoToExpose[] GetEvents(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override EventInfoToExpose[] GetEvents() => throw new NotSupportedException();

        public override FieldInfoToExpose GetField(string name, BindingFlags bindingAttr) => throw new NotSupportedException();

        public override FieldInfoToExpose[] GetFields(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override int GetHashCode() => FullName.GetHashCode() | 12345;

        public override TypeToExpose GetInterface(string name, bool ignoreCase) => throw new NotSupportedException();

        public override TypeToExpose[] GetInterfaces() => throw new NotSupportedException();

        public override MemberInfoToExpose[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) => throw new NotSupportedException();

        public override MemberInfoToExpose[] GetMembers(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override MethodInfoToExpose[] GetMethods(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override TypeToExpose GetNestedType(string name, BindingFlags bindingAttr) => throw new NotSupportedException();

        public override TypeToExpose[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override PropertyInfoToExpose[] GetProperties(BindingFlags bindingAttr) => throw new NotSupportedException();

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => throw new NotSupportedException();
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

        public virtual GenericParameterAttributes GenericParameterAttributes => throw new NotSupportedException();

        public virtual IEnumerable<TypeToExpose> ImplementedInterfaces => GetInterfaces();
    }
#endif
}
