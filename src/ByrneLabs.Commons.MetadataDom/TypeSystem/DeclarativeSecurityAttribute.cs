using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class DeclarativeSecurityAttribute : RuntimeCodeElement, ICodeElementWithTypedHandle<DeclarativeSecurityAttributeHandle, System.Reflection.Metadata.DeclarativeSecurityAttribute>
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _permissionSet;

        internal DeclarativeSecurityAttribute(DeclarativeSecurityAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetDeclarativeSecurityAttribute(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            Action = RawMetadata.Action;
            _permissionSet = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.PermissionSet)));
        }
        public DeclarativeSecurityAction Action { get; }
        public CodeElement Parent => _parent.Value;
        public Blob PermissionSet => _permissionSet.Value;

        public System.Reflection.Metadata.DeclarativeSecurityAttribute RawMetadata { get; }

        public DeclarativeSecurityAttributeHandle MetadataHandle { get; }
    }
}
