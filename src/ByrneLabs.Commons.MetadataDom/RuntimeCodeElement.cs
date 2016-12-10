using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class RuntimeCodeElement : CodeElement
    {
        internal RuntimeCodeElement(object key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader;
    }
}
