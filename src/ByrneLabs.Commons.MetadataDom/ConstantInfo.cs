using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeToExpose = System.Type;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using FieldInfoToExpose = System.Reflection.FieldInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ConstantInfo
    {
        public abstract TypeToExpose ConstantType { get; }

        public abstract object Value { get; }

        public FieldInfoToExpose ParentField => Parent as FieldInfo;

        public ParameterInfoToExpose ParentParameter => Parent as ParameterInfo;

        public PropertyInfoToExpose ParentProperty => Parent as PropertyInfo;

        protected abstract object Parent { get; }

        public override string ToString() => $"({GetType().FullName}) {(Parent as IMemberInfo).FullName}";
    }
}
