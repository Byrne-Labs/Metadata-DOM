using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class RuntimeCodeElement : CodeElement
    {
        internal RuntimeCodeElement(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            if (key.Handle.HasValue)
            {
                DowncastMetadataHandle = key.Handle.Value;
                MetadataToken = DowncastMetadataHandle.GetHashCode();
            }
        }

        internal RuntimeCodeElement(Handle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            if(metadataHandle.IsNil)
            {
                throw new ArgumentException("Cannot have nil handle", nameof(metadataHandle));
            }
            DowncastMetadataHandle = metadataHandle;
            MetadataToken = metadataHandle.GetHashCode();
        }

        public Handle? DowncastMetadataHandle { get; }

        public virtual int MetadataToken { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader;
    }
}
