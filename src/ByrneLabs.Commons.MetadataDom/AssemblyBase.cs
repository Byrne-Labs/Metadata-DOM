using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name.FullName}")]
    public abstract class AssemblyBase : RuntimeCodeElement, IAssembly
    {
        internal AssemblyBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public abstract IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes { get; }

        public abstract IEnumerable<TypeBase> DefinedTypes { get; }

        public abstract IMethod EntryPoint { get; }

        public abstract AssemblyHashAlgorithm HashAlgorithm { get; }

        public abstract IEnumerable<IAssembly> ReferencedAssemblies { get; }

        public string FullName => Name.FullName;

        public abstract IEnumerable<CustomAttribute> CustomAttributes { get; }

        public abstract AssemblyFlags Flags { get; }

        public abstract AssemblyName Name { get; }
    }

    public abstract class AssemblyBase<TAssemblyBase, THandle, TToken> : AssemblyBase, ICodeElementWithTypedHandle<THandle, TToken> where TAssemblyBase : AssemblyBase<TAssemblyBase, THandle, TToken>
    {
        internal AssemblyBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        internal AssemblyBase(THandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<TAssemblyBase>(metadataHandle), metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = (TToken) MetadataState.GetTokenForHandle(metadataHandle);
        }

        public TToken RawMetadata { get; }

        public THandle MetadataHandle { get; }
    }
}
