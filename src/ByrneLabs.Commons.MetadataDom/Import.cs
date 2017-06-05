using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class Import
    {
        public abstract string Alias { get; }

        public abstract AssemblyName TargetAssembly { get; }

        public abstract string TargetNamespace { get; }

        public abstract TypeInfo TargetType { get; }

        public override string ToString() => $"{Alias} = {TargetNamespace}";
    }
}
