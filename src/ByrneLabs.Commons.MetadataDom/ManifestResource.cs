using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource" />
    //[PublicAPI]
    public class ManifestResource : RuntimeCodeElement, ICodeElementWithTypedHandle<ManifestResourceHandle, System.Reflection.Metadata.ManifestResource>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;

        internal ManifestResource(ManifestResourceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetManifestResource(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _implementation = MetadataState.GetLazyCodeElement(RawMetadata.Implementation);
            Offset = RawMetadata.Offset;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Attributes" />
        public ManifestResourceAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Implementation" />
        /// <summary><see cref="AssemblyFile" />, <see cref="AssemblyReference" />, or null.</summary>
        /// <remarks>Corresponds to Implementation field of ManifestResource table in ECMA-335 Standard. If null, then
        ///     <see cref="Offset" /> is an offset in the PE image that contains the metadata, starting from the Resource entry in the CLI header.</remarks>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ManifestResource.Offset" />
        public long Offset { get; }

        public System.Reflection.Metadata.ManifestResource RawMetadata { get; }

        public ManifestResourceHandle MetadataHandle { get; }
    }
}
