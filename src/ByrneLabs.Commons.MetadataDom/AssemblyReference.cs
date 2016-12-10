using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference" />
    [PublicAPI]
    public class AssemblyReference : RuntimeCodeElement, ICodeElementWithHandle<AssemblyReferenceHandle, System.Reflection.Metadata.AssemblyReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;
        private readonly Lazy<Blob> _publicKeyOrToken;

        internal AssemblyReference(AssemblyReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetAssemblyReference(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Culture = AsString(MetadataToken.Culture);
            Flags = MetadataToken.Flags;
            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.HashValue)));
            _publicKeyOrToken = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.PublicKeyOrToken)));
            Version = MetadataToken.Version;
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Culture" />
        public string Culture { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Flags" />
        public AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.HashValue" />
        public Blob HashValue => _hashValue.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.PublicKeyOrToken" />
        public Blob PublicKeyOrToken => _publicKeyOrToken.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Version" />
        public Version Version { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public AssemblyReferenceHandle MetadataHandle { get; }

        public System.Reflection.Metadata.AssemblyReference MetadataToken { get; }
    }
}
