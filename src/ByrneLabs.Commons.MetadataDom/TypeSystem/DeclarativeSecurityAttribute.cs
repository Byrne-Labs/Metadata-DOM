using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class DeclarativeSecurityAttribute : SimpleCodeElement
    {
        private readonly Lazy<object> _parent;
        private readonly Lazy<Blob> _permissionSet;

        internal DeclarativeSecurityAttribute(DeclarativeSecurityAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetDeclarativeSecurityAttribute(metadataHandle);
            _parent = new Lazy<object>(() => MetadataState.GetCodeElement(RawMetadata.Parent));
            Action = RawMetadata.Action;
            _permissionSet = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.PermissionSet, false);
        }

        public DeclarativeSecurityAction Action { get; }

        public DeclarativeSecurityAttributeHandle MetadataHandle { get; }

        public object Parent => _parent.Value;

        public Blob PermissionSet => _permissionSet.Value;

        public System.Reflection.Metadata.DeclarativeSecurityAttribute RawMetadata { get; }
    }
}
