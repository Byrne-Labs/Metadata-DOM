﻿using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class DebugCodeElement : CodeElement
    {
        internal DebugCodeElement(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal DebugCodeElement(Handle metadataHandle, MetadataState metadataState) : this(new CodeElementKey(metadataHandle), metadataState)
        {
        }

        protected override sealed MetadataReader Reader => MetadataState.PdbReader;
    }
}
