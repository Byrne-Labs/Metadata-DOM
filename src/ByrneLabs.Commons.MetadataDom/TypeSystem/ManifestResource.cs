using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
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
        public ManifestResourceAttributes Attributes { get; }
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;
        public CodeElement Implementation => _implementation.Value;
        public string Name { get; }
        public long Offset { get; }

        public System.Reflection.Metadata.ManifestResource RawMetadata { get; }

        public ManifestResourceHandle MetadataHandle { get; }
    }
}
