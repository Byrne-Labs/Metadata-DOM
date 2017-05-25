using System.Diagnostics.CodeAnalysis;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public abstract class ModuleBase : Module, IManagedCodeElement
    {
        internal ModuleBase(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            MetadataState = metadataState;
        }

        public override string FullyQualifiedName => Name;

        public override int MetadataToken => Key.Handle.GetHashCode();

        public string ScopedName { get; protected set; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }

    public abstract class ModuleBase<TModuleBase, THandle, TToken> : ModuleBase where TModuleBase : ModuleBase
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

        public THandle MetadataHandle { get; }

        public TToken RawMetadata { get; }
    }
}
