using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ImportScope : DebugCodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<ImportDefinition>> _imports;
        private readonly Lazy<Blob> _importsBlob;
        private readonly Lazy<ImportScope> _parent;

        internal ImportScope(ImportScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var importScope = Reader.GetImportScope(metadataHandle);
            _parent = new Lazy<ImportScope>(() => GetCodeElement<ImportScope>(importScope.Parent));
            _importsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importScope.ImportsBlob)));
            _imports = new Lazy<IEnumerable<ImportDefinition>>(() => GetCodeElements<ImportDefinition>(importScope.GetImports().Select(import =>new HandlelessCodeElementKey<ImportDefinition>(import))));
        }

        public IEnumerable<ImportDefinition> Imports => _imports.Value;

        public Blob ImportsBlob => _importsBlob.Value;

        public ImportScope Parent => _parent.Value;
    }
}
