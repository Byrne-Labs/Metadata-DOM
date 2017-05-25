using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class AssemblyFile : RuntimeCodeElement, ICodeElementWithTypedHandle<AssemblyFileHandle, System.Reflection.Metadata.AssemblyFile>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;

        internal AssemblyFile(AssemblyFileHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetAssemblyFile(metadataHandle);
            Name = AsString(RawMetadata.Name);
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.HashValue)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            ContainsMetadata = RawMetadata.ContainsMetadata;
        }
        public bool ContainsMetadata { get; }
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;
        public Blob HashValue => _hashValue.Value;
        public string Name { get; }

        public System.Reflection.Metadata.AssemblyFile RawMetadata { get; }

        public AssemblyFileHandle MetadataHandle { get; }
    }
}
