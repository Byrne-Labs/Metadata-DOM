using System.Collections.Generic;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using CustomAttributeTypedArgumentToExpose = System.Reflection.CustomAttributeTypedArgument;
using CustomAttributeNamedArgumentToExpose = System.Reflection.CustomAttributeNamedArgument;

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
        public abstract ConstructorInfoToExpose Constructor { get; }

        public abstract IList<CustomAttributeTypedArgumentToExpose> ConstructorArguments { get; }

        public abstract IList<CustomAttributeNamedArgumentToExpose> NamedArguments { get; }
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class CustomAttributeData : System.Reflection.CustomAttributeData
    {
    }
#else
    public abstract partial class CustomAttributeData
    {
        public TypeToExpose AttributeType => Constructor.DeclaringType;
    }
#endif
}
