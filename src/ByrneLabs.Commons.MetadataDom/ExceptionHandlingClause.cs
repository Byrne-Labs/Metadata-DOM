#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class ExceptionHandlingClause
    {
        public abstract Type CatchType { get; }

        public abstract int FilterOffset { get; }

        public abstract ExceptionHandlingClauseOptions Flags { get; }

        public abstract int HandlerLength { get; }

        public abstract int HandlerOffset { get; }

        public abstract int TryLength { get; }

        public abstract int TryOffset { get; }
    }
}
#endif
