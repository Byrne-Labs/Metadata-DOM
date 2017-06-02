using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;

#endif
namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class LocalConstantInfo
    {
        public abstract TypeInfoToExpose ConstantType { get; }

        public abstract string Name { get; }

        public abstract object Value { get; }

        public override string ToString() => $"({GetType().FullName}) {Name}";
    }
}
