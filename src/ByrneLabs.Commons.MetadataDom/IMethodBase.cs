using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMethodBase : IMember
    {
        bool ContainsGenericParameters { get; }

        IEnumerable<TypeBase> GenericArguments { get; }

        bool IsConstructor { get; }

        bool IsGenericMethod { get; }

        IEnumerable<IParameter> Parameters { get; }
    }
}
