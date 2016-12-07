using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile" />
    public class AssemblyFile : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;
        private readonly Lazy<string> _name;

        internal AssemblyFile(AssemblyFileHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var assemblyFile = Reader.GetAssemblyFile(metadataHandle);
            _name = new Lazy<string>(() => AsString(assemblyFile.Name));
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(assemblyFile.HashValue)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyFile.GetCustomAttributes()));
            ContainsMetadata = assemblyFile.ContainsMetadata;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public bool ContainsMetadata { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.HashValue" />
        public Blob HashValue => _hashValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyFile.ContainsMetadata" />
        public string Name => _name.Value;
    }
}
