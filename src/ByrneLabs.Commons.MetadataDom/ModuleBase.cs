﻿using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class ModuleBase : RuntimeCodeElement, IModule
    {
        internal ModuleBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public abstract ImmutableArray<CustomAttribute> CustomAttributes { get; }

        public abstract IAssembly Assembly { get; }

        public string ScopedName { get; protected set; }
    }

    public abstract class ModuleBase<TModuleBase, THandle, TToken> : ModuleBase, ICodeElementWithTypedHandle<THandle, TToken> where TModuleBase : ModuleBase<TModuleBase, THandle, TToken>
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ModuleBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal ModuleBase(THandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<TModuleBase>(metadataHandle), metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(metadataHandle);
        }

        public TToken RawMetadata { get; }

        public THandle MetadataHandle { get; }
    }
}
