using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMethod : IMethodBase
    {
        TypeBase ReturnType { get; }
    }
}
