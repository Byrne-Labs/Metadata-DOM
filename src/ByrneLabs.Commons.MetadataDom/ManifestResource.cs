using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource" />
    [PublicAPI]
    public class ManifestResource : RuntimeCodeElement, ICodeElementWithHandle<ManifestResourceHandle, System.Reflection.Metadata.ManifestResource>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;

        internal ManifestResource(ManifestResourceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetManifestResource(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _implementation = GetLazyCodeElementWithHandle(MetadataToken.Implementation);
            Offset = MetadataToken.Offset;
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Attributes" />
        public ManifestResourceAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Implementation" />
        /// <summary><see cref="AssemblyFile" />, <see cref="AssemblyReference" />, or null.</summary>
        /// <remarks>Corresponds to Implementation field of ManifestResource table in ECMA-335 Standard. If null, then
        ///     <see cref="Offset" /> is an offset in the PE image that contains the metadata, starting from the Resource entry in the CLI header.</remarks>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Offset" />
        public long Offset { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ManifestResourceHandle MetadataHandle { get; }

        public System.Reflection.Metadata.ManifestResource MetadataToken { get; }
    }
}
