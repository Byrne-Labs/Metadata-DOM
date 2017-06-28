using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public interface IMemberInfo
    {
        BindingFlags BindingFlags { get; }

        IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get; }

        string FullName { get; }

        bool IsCompilerGenerated { get; }

        bool IsSpecialName { get; }

        MemberTypes MemberType { get; }

        int MetadataToken { get; }

        System.Reflection.Module Module { get; }

        string Name { get; }

        IEnumerable<SequencePoint> SequencePoints { get; }

        string SourceCode { get; }

        string TextSignature { get; }

        IList<System.Reflection.CustomAttributeData> GetCustomAttributesData();
    }
}
