using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class RuntimeCodeElement : CodeElement
    {
        internal RuntimeCodeElement(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal RuntimeCodeElement(Handle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
         protected override sealed MetadataReader Reader => MetadataState.AssemblyReader;
    }
}
