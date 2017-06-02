using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Linq;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using ModuleToExpose = System.Reflection.Module;
using AssemblyToExpose = System.Reflection.Assembly;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using MemberInfoToExpose = ByrneLabs.Commons.MetadataDom.MemberInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class EventInfo
    {
        public bool IsPublic => AddMethod?.IsPublic != false && RemoveMethod?.IsPublic != false && RaiseMethod?.IsPublic != false;

        public bool IsStatic => AddMethod?.IsStatic != false && RemoveMethod?.IsStatic != false && RaiseMethod?.IsStatic != false;

        public override MemberTypes MemberType => MemberTypes.Event;

        internal BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public override string ToString() => $"({GetType().FullName}) {FullName}";
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class EventInfo : System.Reflection.EventInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract bool IsSpecialName { get; }

        public abstract string TextSignature { get; }

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;
    }
#else
    public abstract partial class EventInfo : MemberInfoToExpose
    {
        public abstract TypeToExpose EventHandlerType { get; }

        public abstract bool IsMulticast { get; }

        public abstract EventAttributes Attributes { get; }

        public virtual MethodInfoToExpose AddMethod => GetAddMethod(true);

        public virtual MethodInfoToExpose RaiseMethod => GetRaiseMethod(true);

        public virtual MethodInfoToExpose RemoveMethod => GetRemoveMethod(true);

        public abstract MethodInfoToExpose GetAddMethod(bool nonPublic);

        public abstract MethodInfoToExpose GetRaiseMethod(bool nonPublic);

        public abstract MethodInfoToExpose GetRemoveMethod(bool nonPublic);

        public MethodInfoToExpose GetAddMethod() => GetAddMethod(false);

        public MethodInfoToExpose GetRaiseMethod() => GetRaiseMethod(false);

        public MethodInfoToExpose GetRemoveMethod() => GetRemoveMethod(false);
    }
#endif
}
