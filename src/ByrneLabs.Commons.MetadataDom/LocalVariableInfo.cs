using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class LocalVariableInfo : System.Reflection.LocalVariableInfo
    {
        public abstract int Index { get; }

        public abstract bool IsDebuggerHidden { get; }

        public abstract string Name { get; }
    }
}
