using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class AssemblyFile : RuntimeCodeElement, ICodeElementWithHandle<AssemblyFileHandle, System.Reflection.Metadata.AssemblyFile>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;

        internal AssemblyFile(AssemblyFileHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetAssemblyFile(metadataHandle);
            Name = AsString(MetadataToken.Name);
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.HashValue)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            ContainsMetadata = MetadataToken.ContainsMetadata;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public bool ContainsMetadata { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.HashValue" />
        public Blob HashValue => _hashValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public string Name { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public AssemblyFileHandle MetadataHandle { get; }

        public System.Reflection.Metadata.AssemblyFile MetadataToken { get; }
    }
}
