using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class CustomAttributeData : System.Reflection.CustomAttributeData
    {
        internal static object[] GetCustomAttributes(IMemberInfo member, bool inherit) => throw NotSupportedHelper.FutureVersion();

        internal static object[] GetCustomAttributes(IMemberInfo member, Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        internal static bool IsDefined(IMemberInfo member, Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();
    }
}
