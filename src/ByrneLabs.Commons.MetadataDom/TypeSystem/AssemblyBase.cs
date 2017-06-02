using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public abstract class AssemblyBase : Assembly, IManagedCodeElement
    {
        internal AssemblyBase(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            MetadataState = metadataState;
            metadataState.CacheCodeElement(this, key);
        }

        public override SecurityRuleSet SecurityRuleSet => throw NotSupportedHelper.FutureVersion();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        protected override void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                MetadataState.Dispose();
            }
        }
    }

    public abstract class AssemblyBase<TAssemblyBase, THandle, TToken> : AssemblyBase where TAssemblyBase : AssemblyBase<TAssemblyBase, THandle, TToken>
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal AssemblyBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal AssemblyBase(THandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<TAssemblyBase>(metadataHandle), metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(metadataHandle);
        }

        public THandle MetadataHandle { get; }

        public TToken RawMetadata { get; }
    }
}
