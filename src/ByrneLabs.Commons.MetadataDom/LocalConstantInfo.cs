using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class LocalConstantInfo
    {
        public abstract System.Reflection.TypeInfo ConstantType { get; }

        public abstract string Name { get; }

        public abstract object Value { get; }

        public override string ToString() => Name;
    }
}
