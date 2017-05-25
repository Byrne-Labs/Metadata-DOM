using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class Import
    {
        public abstract string Alias { get; }

        public abstract ImportKind Kind { get; }

        public abstract AssemblyName TargetAssembly { get; }

        public abstract string TargetNamespace { get; }

        public abstract TypeInfo TargetType { get; }
    }
}
