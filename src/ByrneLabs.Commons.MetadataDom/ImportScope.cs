using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ImportScope : DebugCodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<ImportDefinition>> _imports;
        private readonly Lazy<Blob> _importsBlob;
        private readonly Lazy<ImportScope> _parent;

        internal ImportScope(ImportScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var importScope = Reader.GetImportScope(metadataHandle);
            _parent = new Lazy<ImportScope>(() => GetCodeElement<ImportScope>(importScope.Parent));
            _importsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importScope.ImportsBlob)));
            _imports = new Lazy<IReadOnlyList<ImportDefinition>>(() => GetCodeElements<ImportDefinition>(importScope.GetImports()));
        }

        public IReadOnlyList<ImportDefinition> Imports => _imports.Value;

        public Blob ImportsBlob => _importsBlob.Value;

        public ImportScope Parent => _parent.Value;
    }
}
