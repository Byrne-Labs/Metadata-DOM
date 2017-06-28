using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class ImportScope : MetadataDom.ImportScope, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<ImportDefinition>> _imports;
        private readonly Lazy<Blob> _importsBlob;
        private readonly Lazy<ImportScope> _parent;

        internal ImportScope(ImportScopeHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<ImportScope>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.PdbReader.GetImportScope(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement<ImportScope>(RawMetadata.Parent);
            _importsBlob = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.ImportsBlob, true);
            _imports = MetadataState.GetLazyCodeElements<ImportDefinition>(RawMetadata.GetImports());
        }

        public override IEnumerable<Import> Imports => _imports.Value;

        public Blob ImportsBlob => _importsBlob.Value;

        public ImportScopeHandle MetadataHandle { get; }

        public override MetadataDom.ImportScope Parent => _parent.Value;

        public System.Reflection.Metadata.ImportScope RawMetadata { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
