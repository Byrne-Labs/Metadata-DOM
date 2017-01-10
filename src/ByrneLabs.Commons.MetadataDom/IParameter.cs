using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IParameter : IMember
    {
        IEnumerable<CustomAttribute> CustomAttributes { get; }

        bool HasDefaultValue { get; }

        bool IsIn { get; }

        bool IsOptional { get; }

        bool IsOut { get; }

        bool IsRetval { get; }

        IMember Member { get; }

        TypeBase ParameterType { get; }

        int Position { get; }
    }
}
