using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class MethodDebugInformation
    {
        public abstract Document Document { get; }

        public abstract MethodBase Method { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public abstract MethodBase StateMachineKickoffMethod { get; }
    }
}
