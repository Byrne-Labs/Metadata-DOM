using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class DeclarativeSecurityAttribute : CodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _permissionSet;

        internal DeclarativeSecurityAttribute(DeclarativeSecurityAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var declarativeSecurityAttribute = Reader.GetDeclarativeSecurityAttribute(metadataHandle);
            _parent = new Lazy<CodeElement>(() => GetCodeElement(declarativeSecurityAttribute.Parent));
            Action = declarativeSecurityAttribute.Action;
            _permissionSet = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(declarativeSecurityAttribute.PermissionSet)));
        }

        public DeclarativeSecurityAction Action { get; }

        public CodeElement Parent => _parent.Value;

        public Blob PermissionSet => _permissionSet.Value;
    }
}
