using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Linq;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using ModuleToExpose = System.Reflection.Module;

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

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class FieldInfo
    {
        public override MemberTypes MemberType => MemberTypes.Field;

        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public abstract object GetRawConstantValue();

        public override string ToString() => FullName;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class FieldInfo : System.Reflection.FieldInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract string TextSignature { get; }

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;
    }
#else
    public abstract partial class FieldInfo : MemberInfo
    {
        public abstract FieldAttributes Attributes { get; }

        public abstract TypeToExpose FieldType { get; }

        public bool IsAssembly => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly;

        public bool IsFamily => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Family;

        public bool IsFamilyAndAssembly => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamANDAssem;

        public bool IsFamilyOrAssembly => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamORAssem;

        public bool IsInitOnly => (Attributes & FieldAttributes.InitOnly) != 0;

        public bool IsLiteral => (Attributes & FieldAttributes.Literal) != 0;

        public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

        public bool IsPinvokeImpl => (Attributes & FieldAttributes.PinvokeImpl) != 0;

        public bool IsPrivate => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Private;

        public bool IsPublic => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;

        public override bool IsSpecialName => (Attributes & FieldAttributes.SpecialName) != 0;

        public bool IsStatic => (Attributes & FieldAttributes.Static) != 0;
    }

#endif
}
