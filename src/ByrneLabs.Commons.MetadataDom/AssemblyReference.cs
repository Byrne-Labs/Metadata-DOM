using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class AssemblyReference : CodeElementWithHandle
    {
        private readonly Lazy<string> _culture;
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;
        private readonly Lazy<string> _name;
        private readonly Lazy<Blob> _publicKeyOrToken;

        internal AssemblyReference(AssemblyReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var assemblyReference = Reader.GetAssemblyReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(assemblyReference.Name));
            _culture = new Lazy<string>(() => AsString(assemblyReference.Culture));
            Flags = assemblyReference.Flags;
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(assemblyReference.HashValue)));
            _publicKeyOrToken = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(assemblyReference.PublicKeyOrToken)));
            Version = assemblyReference.Version;
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyReference.GetCustomAttributes()));
        }

        public string Culture => _culture.Value;

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public AssemblyFlags Flags { get; }

        public Blob HashValue => _hashValue.Value;

        public string Name => _name.Value;

        public Blob PublicKeyOrToken => _publicKeyOrToken.Value;

        public Version Version { get; }
    }
}
