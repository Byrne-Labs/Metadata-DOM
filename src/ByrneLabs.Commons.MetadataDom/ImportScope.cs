using System.Collections.Immutable;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ImportScope
    {
        public abstract ImmutableArray<Import> Imports { get; }

        public abstract ImportScope Parent { get; }
    }
}
