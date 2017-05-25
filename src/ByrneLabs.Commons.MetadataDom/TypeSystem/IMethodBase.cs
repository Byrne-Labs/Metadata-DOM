using System.Collections.Immutable;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public interface IMethodBase : IMember
    {
        bool IsConstructor { get; }

        bool IsGenericMethod { get; }

        ImmutableArray<Parameter> Parameters { get; }

        ImmutableArray<GenericParameter> GenericTypeParameters { get; }

    }
}
