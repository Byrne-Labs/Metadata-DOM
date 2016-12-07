using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource" />
    public class ManifestResource : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<string> _name;

        internal ManifestResource(ManifestResourceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var manifestResource = Reader.GetManifestResource(metadataHandle);
            _name = new Lazy<string>(() => AsString(manifestResource.Name));
            Attributes = manifestResource.Attributes;
            _implementation = new Lazy<CodeElement>(() => GetCodeElement(manifestResource.Implementation));
            Offset = manifestResource.Offset;
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(manifestResource.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Attributes" />
        public ManifestResourceAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Implementation" />
        /// <summary>
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.AssemblyFile" />, <see cref="T:ByrneLabs.Commons.MetadataDom.AssemblyReference" />, or null.</summary>
        /// <remarks>Corresponds to Implementation field of ManifestResource table in ECMA-335 Standard. If null, then
        ///     <see cref="P:ByrneLabs.Commons.MetadataDom.ManifestResource.Offset" /> is an offset in the PE image that contains the metadata, starting from the Resource entry in the CLI header.</remarks>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Offset" />
        public long Offset { get; }
    }
}
