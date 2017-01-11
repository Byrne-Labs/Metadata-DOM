using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMethodBase : IMember
    {
        IEnumerable<TypeBase> GenericArguments { get; }

        bool IsConstructor { get; }

        bool IsGenericMethod { get; }

        IEnumerable<IParameter> Parameters { get; }
    }
}
