using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ConstantInfo
    {
        public abstract Type ConstantType { get; }

        public abstract object Value { get; }

        public FieldInfo ParentField => Parent as FieldInfo;

        public ParameterInfo ParentParameter => Parent as ParameterInfo;

        public PropertyInfo ParentProperty => Parent as PropertyInfo;

        protected abstract object Parent { get; }

        public override string ToString() => (Parent as IMemberInfo).FullName;
    }
}
