using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class CustomDebugInformation : DebugCodeElement, ICodeElementWithTypedHandle<CustomDebugInformationHandle, System.Reflection.Metadata.CustomDebugInformation>
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetCustomDebugInformation(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            Kind = AsGuid(RawMetadata.Kind);
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.Value)));
        }
        public Guid Kind { get; }
        public CodeElement Parent => _parent.Value;
        public Blob Value => _value.Value;

        public System.Reflection.Metadata.CustomDebugInformation RawMetadata { get; }

        public CustomDebugInformationHandle MetadataHandle { get; }
    }
}
