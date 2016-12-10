using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class DebugCodeElement : CodeElement
    {
        internal DebugCodeElement(object key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        protected override sealed MetadataReader Reader => MetadataState.PdbReader;
    }
}
