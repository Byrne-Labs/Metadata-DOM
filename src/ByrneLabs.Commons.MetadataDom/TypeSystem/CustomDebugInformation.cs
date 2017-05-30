using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class CustomDebugInformation : SimpleCodeElement
    {
        private readonly Lazy<object> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.PdbReader.GetCustomDebugInformation(metadataHandle);
            _parent = new Lazy<object>(() => MetadataState.GetCodeElement(RawMetadata.Parent));
            Kind = MetadataState.PdbReader.GetGuid(RawMetadata.Kind);
            _value = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.Value, true);
        }

        public Guid Kind { get; }

        public CustomDebugInformationHandle MetadataHandle { get; }

        public object Parent => _parent.Value;

        public System.Reflection.Metadata.CustomDebugInformation RawMetadata { get; }

        public Blob Value => _value.Value;
    }
}
