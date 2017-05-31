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
    public abstract class MethodDebugInformation
    {
        public abstract Document Document { get; }

        public abstract string FullName { get; }

        public abstract MethodBaseToExpose Method { get; }

        public abstract string Name { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public abstract MethodBaseToExpose StateMachineKickoffMethod { get; }

        public abstract string TextSignature { get; }
    }
}
