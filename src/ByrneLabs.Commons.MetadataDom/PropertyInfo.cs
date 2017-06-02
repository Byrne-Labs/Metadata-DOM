using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Linq;
using System.Globalization;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class PropertyInfo
    {
        public abstract ConstantInfo DefaultValue { get; }

        public abstract bool IsIndexer { get; }

        public bool IsPublic => GetMethod?.IsPublic != false && SetMethod?.IsPublic != false;

        public bool IsStatic => GetMethod?.IsStatic != false && SetMethod?.IsStatic != false;

        internal BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public override string ToString() => $"({GetType().FullName}) {FullName}";
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class PropertyInfo : System.Reflection.PropertyInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract bool IsSpecialName { get; }

        public abstract ModuleToExpose Module { get; }

        public abstract string TextSignature { get; }

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();
    }
#else
    public abstract partial class PropertyInfo : MemberInfo
    {
        public abstract MethodInfoToExpose GetMethod { get; }

        public abstract MethodInfoToExpose SetMethod { get; }

        public abstract PropertyAttributes Attributes { get; }

        public abstract bool CanRead { get; }

        public abstract bool CanWrite { get; }

        public abstract TypeToExpose PropertyType { get; }

        public abstract MethodInfoToExpose[] GetAccessors(bool nonPublic);

        public abstract MethodInfoToExpose GetGetMethod(bool nonPublic);

        public abstract ParameterInfoToExpose[] GetIndexParameters();

        public abstract MethodInfoToExpose GetSetMethod(bool nonPublic);
    }
#endif
}
