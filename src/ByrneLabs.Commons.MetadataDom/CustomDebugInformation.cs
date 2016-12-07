using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class CustomDebugInformation : DebugCodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var customDebugInformation = Reader.GetCustomDebugInformation(metadataHandle);
            _parent = new Lazy<CodeElement>(() => GetCodeElement(customDebugInformation.Parent));
            Kind = AsGuid(customDebugInformation.Kind);
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(customDebugInformation.Value)));
        }

        public Guid Kind { get; }

        public CodeElement Parent => _parent.Value;

        public Blob Value => _value.Value;
    }
}
