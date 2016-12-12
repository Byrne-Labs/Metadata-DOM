using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation" />
    [PublicAPI]
    public class CustomDebugInformation : DebugCodeElement, ICodeElementWithHandle<CustomDebugInformationHandle, System.Reflection.Metadata.CustomDebugInformation>
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetCustomDebugInformation(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            Kind = AsGuid(MetadataToken.Kind);
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Kind" />
        public Guid Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Value" />
        public Blob Value => _value.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public CustomDebugInformationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.CustomDebugInformation MetadataToken { get; }
    }
}
