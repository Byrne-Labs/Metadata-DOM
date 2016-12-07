using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.Action" />
        public DeclarativeSecurityAction Action { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.PermissionSet" />
        public Blob PermissionSet => _permissionSet.Value;
    }
}
