using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class LocalScope
    {
        public abstract IEnumerable<LocalScope> Children { get; }

        public abstract int EndOffset { get; }

        public abstract ImportScope ImportScope { get; }

        public abstract int Length { get; }

        public abstract IEnumerable<LocalConstantInfo> LocalConstants { get; }

        public abstract IEnumerable<LocalVariableInfo> LocalVariables { get; }

        public abstract MethodBase Method { get; }

        public abstract int StartOffset { get; }
    }
}
