using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Collections.Generic;
using System.Linq;
using TypeInfoToExpose = System.Reflection.TypeInfo;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using ModuleToExpose = System.Reflection.Module;
using EventInfoToExpose = System.Reflection.EventInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class MethodInfo
    {
        public abstract EventInfoToExpose RelatedEvent { get; }

        public abstract PropertyInfoToExpose RelatedProperty { get; }

        public abstract TypeInfoToExpose ReturnType { get; }

        public bool IsEventAdder => RelatedEvent != null && RelatedEvent.AddMethod == this;

        public bool IsEventRaiser => RelatedEvent != null && RelatedEvent.RaiseMethod == this;

        public bool IsEventRemover => RelatedEvent != null && RelatedEvent.RemoveMethod == this;

        public bool IsPropertyGetter => RelatedProperty != null && RelatedProperty.GetMethod == this;

        public bool IsPropertySetter => RelatedProperty != null && RelatedProperty.SetMethod == this;

        public override MemberTypes MemberType => MemberTypes.Method;

        internal BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class MethodInfo : MethodInfoToExpose, IMemberInfo
    {
        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();

        public abstract string FullName { get; }

        public abstract ModuleToExpose Module { get; }

        public abstract IEnumerable<ParameterInfoToExpose> Parameters { get; }

        public abstract string TextSignature { get; }

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public override ParameterInfoToExpose[] GetParameters() => Parameters.ToArray();
    }
#else
    public abstract partial class MethodInfo : MethodBase
    {
    }
#endif
}
