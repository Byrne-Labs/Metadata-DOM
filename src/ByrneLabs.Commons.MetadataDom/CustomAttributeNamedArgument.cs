#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public struct CustomAttributeNamedArgument
    {
        internal CustomAttributeNamedArgument(MemberInfo memberInfo, object value)
        {
            MemberInfo = memberInfo;
            Type memberType;

            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (memberInfo is FieldInfo)
            {
                memberType = ((FieldInfo) memberInfo).FieldType;
            }
            else if (memberInfo is PropertyInfo)
            {
                memberType = ((PropertyInfo) memberInfo).PropertyType;
            }
            else
            {
                throw new ArgumentException($"Unexpected member type {memberInfo.GetType().FullName}", nameof(memberInfo));
            }

            TypedValue = new CustomAttributeTypedArgument(memberType, value);
        }

        public MemberInfo MemberInfo { get; }

        public CustomAttributeTypedArgument TypedValue { get; }

        public string MemberName => MemberInfo.Name;

        public bool IsField => MemberInfo is FieldInfo;
    }
}

#endif
