using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class SequencePoint
    {
        public abstract Document Document { get; }

        public abstract int EndColumn { get; }

        public abstract int EndLine { get; }

        public abstract string FullName { get; }

        public abstract bool IsHidden { get; }

        public abstract int Offset { get; }

        public abstract string SourceCode { get; }

        public abstract int StartColumn { get; }

        public abstract int StartLine { get; }

        public virtual string Name => $"{Document.Name}({StartLine}:{StartColumn},{EndLine},{EndColumn})";
    }
}
