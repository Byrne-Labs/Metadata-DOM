using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name.FullName}")]
    public abstract class AssemblyBase : RuntimeCodeElement, IAssembly
    {
        internal AssemblyBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public string FullName => Name.FullName;

        public abstract ImmutableArray<CustomAttribute> CustomAttributes { get; }

        public abstract AssemblyFlags Flags { get; }

        public abstract AssemblyName Name { get; }
    }

    public abstract class AssemblyBase<TAssemblyBase, THandle, TToken> : AssemblyBase, ICodeElementWithTypedHandle<THandle, TToken> where TAssemblyBase : AssemblyBase<TAssemblyBase, THandle, TToken>
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

        public TToken RawMetadata { get; }

        public THandle MetadataHandle { get; }
    }
}
