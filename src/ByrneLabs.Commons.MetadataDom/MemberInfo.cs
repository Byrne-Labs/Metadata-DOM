#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class MemberInfo : IMemberInfo
    {
        public abstract Type DeclaringType { get; }

        public abstract string FullName { get; }

        public abstract bool IsSpecialName { get; }

        public abstract MemberTypes MemberType { get; }

        public abstract int MetadataToken { get; }

        public abstract Module Module { get; }

        public abstract string Name { get; }

        public abstract Type ReflectedType { get; }

        public abstract string TextSignature { get; }

        public IEnumerable<CustomAttributeDataToExpose> CustomAttributes => GetCustomAttributesData();

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType != ReflectedType;

        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();
    }
}

#endif
