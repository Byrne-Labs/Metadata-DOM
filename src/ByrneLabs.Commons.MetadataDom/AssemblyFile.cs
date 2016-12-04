using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class AssemblyFile : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;
        private readonly Lazy<string> _name;

        internal AssemblyFile(AssemblyFileHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var assemblyFile = Reader.GetAssemblyFile(metadataHandle);
            _name = new Lazy<string>(() => AsString(assemblyFile.Name));
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(assemblyFile.HashValue)));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyFile.GetCustomAttributes()));
            ContainsMetadata = assemblyFile.ContainsMetadata;
        }

        public bool ContainsMetadata { get; }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public Blob HashValue => _hashValue.Value;

        public string Name => _name.Value;
    }
}
