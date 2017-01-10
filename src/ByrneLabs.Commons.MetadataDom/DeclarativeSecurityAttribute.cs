using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute" />
    //[PublicAPI]
    public class DeclarativeSecurityAttribute : RuntimeCodeElement, ICodeElementWithHandle<DeclarativeSecurityAttributeHandle, System.Reflection.Metadata.DeclarativeSecurityAttribute>
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _permissionSet;

        internal DeclarativeSecurityAttribute(DeclarativeSecurityAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetDeclarativeSecurityAttribute(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            Action = MetadataToken.Action;
            var permissionSet = Reader.GetBlobBytes(MetadataToken.PermissionSet);
            _permissionSet = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.PermissionSet)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.Action" />
        public DeclarativeSecurityAction Action { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.DeclarativeSecurityAttribute.PermissionSet" />
        public Blob PermissionSet => _permissionSet.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public DeclarativeSecurityAttributeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.DeclarativeSecurityAttribute MetadataToken { get; }
    }
}
