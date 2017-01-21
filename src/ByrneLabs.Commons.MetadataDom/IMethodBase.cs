using System.Collections.Generic;
using System.Collections.Immutable;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMethodBase : IMember
    {
        bool IsConstructor { get; }

        bool IsGenericMethod { get; }

        ImmutableArray<IParameter> Parameters { get; }

        ImmutableArray<GenericParameter> GenericTypeParameters { get; }

    }
}
