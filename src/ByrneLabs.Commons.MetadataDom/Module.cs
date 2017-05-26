using System;
using System.Linq;
using System.Reflection;
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

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class Module
    {
        internal const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public abstract Guid BaseGenerationId { get; }

        public abstract int Generation { get; }

        public abstract Guid GenerationId { get; }

        public abstract bool Manifest { get; }

        public override int MDStreamVersion => throw new NotSupportedException();

        protected abstract FieldInfoToExpose[] GetAllFields();

        protected abstract MethodInfoToExpose[] GetAllMethods();

        private FieldInfoToExpose[] GetFieldsImpl(BindingFlags bindingFlags) => GetAllFields().OfType<FieldInfo>().Where(field => (field.BindingFlags & bindingFlags) == bindingFlags).ToArray();

        private MethodInfoToExpose[] GetMethodsImpl(BindingFlags bindingFlags) => GetAllMethods().Cast<MethodInfo>().Where(method => (method.BindingFlags & bindingFlags) == bindingFlags).ToArray();
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class Module : ModuleToExpose
    {
        public override FieldInfoToExpose[] GetFields(BindingFlags bindingFlags) => GetFieldsImpl(bindingFlags);

        public override MethodInfoToExpose[] GetMethods(BindingFlags bindingFlags) => GetMethodsImpl(bindingFlags);
    }
#else
    public abstract partial class Module : ICustomAttributeProvider
    {
        public abstract AssemblyToExpose Assembly { get; }

        public abstract int MetadataToken { get; }

        public abstract Guid ModuleVersionId { get; }

        public abstract string Name { get; }

        public abstract string ScopeName { get; }

        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();

        public abstract TypeToExpose[] GetTypes();

        public abstract bool IsResource();

        public abstract string FullyQualifiedName { get; }

        public virtual IEnumerable<CustomAttributeDataToExpose> CustomAttributes => GetCustomAttributesData();

        public virtual object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public virtual object[] GetCustomAttributes(System.Type attributeType, bool inherit) => throw new NotSupportedException();

        public virtual FieldInfoToExpose[] GetFields(BindingFlags bindingFlags) => GetFieldsImpl(bindingFlags);

        public FieldInfoToExpose[] GetFields() => GetFields(DefaultLookup);

        public virtual MethodInfoToExpose[] GetMethods(BindingFlags bindingFlags) => GetMethodsImpl(bindingFlags);

        public MethodInfoToExpose[] GetMethods() => GetMethods(DefaultLookup);

        public virtual bool IsDefined(System.Type attributeType, bool inherit) => throw new NotSupportedException();
    }
#endif
}
