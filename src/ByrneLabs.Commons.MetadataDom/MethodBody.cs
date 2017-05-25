#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class MethodBody
    {
        public abstract IList<ExceptionHandlingClause> ExceptionHandlingClauses { get; }

        public abstract bool InitLocals { get; }

        public abstract int LocalSignatureMetadataToken { get; }

        public abstract IList<LocalVariableInfo> LocalVariables { get; }

        public abstract int MaxStackSize { get; }

        public abstract byte[] GetILAsByteArray();
    }
}
#endif
