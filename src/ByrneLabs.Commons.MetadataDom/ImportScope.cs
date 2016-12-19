using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ImportScope" />
    //[PublicAPI]
    public class ImportScope : DebugCodeElement, ICodeElementWithHandle<ImportScopeHandle, System.Reflection.Metadata.ImportScope>
    {
        private readonly Lazy<IEnumerable<ImportDefinition>> _imports;
        private readonly Lazy<Blob> _importsBlob;
        private readonly Lazy<ImportScope> _parent;

        internal ImportScope(ImportScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetImportScope(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement<ImportScope>(MetadataToken.Parent);
            _importsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.ImportsBlob)));
            _imports = MetadataState.GetLazyCodeElements<ImportDefinition>(MetadataToken.GetImports());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.GetImports" />
        public IEnumerable<ImportDefinition> Imports => _imports.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.ImportsBlob" />
        public Blob ImportsBlob => _importsBlob.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.Parent" />
        public ImportScope Parent => _parent.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ImportScopeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.ImportScope MetadataToken { get; }
    }
}
