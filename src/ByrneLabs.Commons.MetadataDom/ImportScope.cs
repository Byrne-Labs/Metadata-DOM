using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ImportScope" />
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
            _imports = new Lazy<IEnumerable<ImportDefinition>>(() => GetCodeElements<ImportDefinition>(importScope.GetImports().Select(import => new HandlelessCodeElementKey<ImportDefinition>(import))));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.GetImports" />
        public IEnumerable<ImportDefinition> Imports => _imports.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.ImportsBlob" />
        public Blob ImportsBlob => _importsBlob.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportScope.Parent" />
        public ImportScope Parent => _parent.Value;
    }
}
