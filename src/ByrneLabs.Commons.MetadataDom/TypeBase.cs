using System.Diagnostics;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("{Namespace,nq}.{Name,nq}")]
    [PublicAPI]
    public abstract class TypeBase : RuntimeCodeElement
    {
        internal TypeBase(object metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public abstract string Name { get; }

        public abstract string Namespace { get; }

        ///<inheritdoc cref="System.Type.FullName"/>
        public abstract string FullName { get; }

    }
}
