using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class ModuleBase : RuntimeCodeElement, IModule
    {
        internal ModuleBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public abstract IEnumerable<CustomAttribute> CustomAttributes { get; }

        public abstract IAssembly Assembly { get; }

        public string Name { get; protected set; }
    }

    public abstract class ModuleBase<TModuleBase, THandle, TToken> : ModuleBase, ICodeElementWithHandle<THandle, TToken> where TModuleBase : ModuleBase<TModuleBase, THandle, TToken>
    {
        internal ModuleBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal ModuleBase(THandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<TModuleBase>(metadataHandle), metadataState)
        {
            MetadataHandle = metadataHandle;
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
        }

        public Handle DowncastMetadataHandle { get; }

        public THandle MetadataHandle { get; }

        public TToken MetadataToken { get; }
    }
}
