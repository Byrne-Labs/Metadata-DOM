using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMethodBase : IMember
    {
        bool IsConstructor { get; }

        bool IsGenericMethod { get; }

        IEnumerable<IParameter> Parameters { get; }

        IEnumerable<GenericParameter> GenericTypeParameters { get; }

    }
}
