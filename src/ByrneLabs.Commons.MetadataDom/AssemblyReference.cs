using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference" />
    public class AssemblyReference : CodeElementWithHandle
    {
        private readonly Lazy<string> _culture;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
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
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyReference.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Culture" />
        public string Culture => _culture.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Flags" />
        public AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.HashValue" />
        public Blob HashValue => _hashValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.PublicKeyOrToken" />
        public Blob PublicKeyOrToken => _publicKeyOrToken.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Version" />
        public Version Version { get; }
    }
}
