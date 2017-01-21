using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public bool ContainsMetadata { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.HashValue" />
        public Blob HashValue => _hashValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public string Name { get; }

        public System.Reflection.Metadata.AssemblyFile RawMetadata { get; }

        public AssemblyFileHandle MetadataHandle { get; }
    }
}
