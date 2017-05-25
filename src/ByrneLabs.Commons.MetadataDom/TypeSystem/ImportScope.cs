using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
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
        public ImmutableArray<ImportDefinition> Imports => _imports.Value;
        public Blob ImportsBlob => _importsBlob.Value;
        public ImportScope Parent => _parent.Value;

        public System.Reflection.Metadata.ImportScope RawMetadata { get; }

        public ImportScopeHandle MetadataHandle { get; }
    }
}
