using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ImportScope" />
    //[PublicAPI]
    public class ImportScope : DebugCodeElement, ICodeElementWithTypedHandle<ImportScopeHandle, System.Reflection.Metadata.ImportScope>
    {
        private readonly Lazy<ImmutableArray<ImportDefinition>> _imports;
        private readonly Lazy<Blob> _importsBlob;
        private readonly Lazy<ImportScope> _parent;

        internal ImportScope(ImportScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetImportScope(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement<ImportScope>(RawMetadata.Parent);
            _importsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.ImportsBlob)));
            _imports = MetadataState.GetLazyCodeElements<ImportDefinition>(RawMetadata.GetImports());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.GetImports" />
        public ImmutableArray<ImportDefinition> Imports => _imports.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.ImportsBlob" />
        public Blob ImportsBlob => _importsBlob.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.Parent" />
        public ImportScope Parent => _parent.Value;

        public System.Reflection.Metadata.ImportScope RawMetadata { get; }

        public ImportScopeHandle MetadataHandle { get; }
    }
}
