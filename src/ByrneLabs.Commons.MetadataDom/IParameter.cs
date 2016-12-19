using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IParameter
    {
        IEnumerable<CustomAttribute> CustomAttributes { get; }

        bool HasDefaultValue { get; }

        bool IsIn { get; }

        bool IsOptional { get; }

        bool IsOut { get; }

        bool IsRetval { get; }

        IMember Member { get; }

        string Name { get; }

        TypeBase ParameterType { get; }

        int Position { get; }

        string TextSignature { get; }
    }
}
