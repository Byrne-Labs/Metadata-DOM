using System.Collections.Generic;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using MethodBaseToExpose = System.Reflection.MethodBase;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;

#endif

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

        public abstract MethodBaseToExpose Method { get; }

        public abstract int StartOffset { get; }
    }
}
