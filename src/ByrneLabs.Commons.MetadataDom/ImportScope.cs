using System.Collections.Generic;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ImportScope
    {
        public abstract IEnumerable<Import> Imports { get; }

        public abstract ImportScope Parent { get; }
    }
}
