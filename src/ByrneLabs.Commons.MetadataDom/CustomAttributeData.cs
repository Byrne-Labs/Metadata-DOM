using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using CustomAttributeTypedArgumentToExpose = System.Reflection.CustomAttributeTypedArgument;
using CustomAttributeNamedArgumentToExpose = System.Reflection.CustomAttributeNamedArgument;
using TypeToExpose = System.Type;

#else
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using CustomAttributeTypedArgumentToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeTypedArgument;
using CustomAttributeNamedArgumentToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeNamedArgument;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class CustomAttributeData
    {
        internal static object[] GetCustomAttributes(IMemberInfo member, bool inherit) => throw NotSupportedHelper.FutureVersion();

        internal static object[] GetCustomAttributes(IMemberInfo member, TypeToExpose attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        internal static bool IsDefined(IMemberInfo member, TypeToExpose attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override string ToString() => AttributeType.Name;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class CustomAttributeData : System.Reflection.CustomAttributeData
    {
    }
#else
    public abstract partial class CustomAttributeData
    {
        public TypeToExpose AttributeType => Constructor.DeclaringType;

        public abstract ConstructorInfoToExpose Constructor { get; }

        public abstract IList<CustomAttributeTypedArgumentToExpose> ConstructorArguments { get; }

        public abstract IList<CustomAttributeNamedArgumentToExpose> NamedArguments { get; }
    }
#endif
}
