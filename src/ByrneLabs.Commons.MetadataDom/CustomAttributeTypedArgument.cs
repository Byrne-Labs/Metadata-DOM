#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public struct CustomAttributeTypedArgument
    {
        internal CustomAttributeTypedArgument(Type argumentType, object value)
        {
            ArgumentType = argumentType;
            Value = value;
        }

        public Type ArgumentType { get; }

        public object Value { get; }
    }
}

#endif
