using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public interface IMemberInfo
    {
        IEnumerable<CustomAttributeDataToExpose> CustomAttributes { get; }

        string FullName { get; }

        bool IsCompilerGenerated { get; }

        bool IsSpecialName { get; }

        MemberTypes MemberType { get; }

        int MetadataToken { get; }

        ModuleToExpose Module { get; }

        string Name { get; }

        string TextSignature { get; }

        IList<CustomAttributeDataToExpose> GetCustomAttributesData();
    }
}
