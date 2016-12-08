using System;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodImport" />
    [PublicAPI]
    public class MethodImport : CodeElementWithoutHandle
    {
        private readonly Lazy<ModuleReference> _module;
        private readonly Lazy<string> _name;

        internal MethodImport(System.Reflection.Metadata.MethodImport methodImport, MetadataState metadataState) : base(new HandlelessCodeElementKey<MethodImport>(methodImport), metadataState)
        {
            Attributes = methodImport.Attributes;
            _name = new Lazy<string>(() => AsString(methodImport.Name));
            _module = new Lazy<ModuleReference>(() => GetCodeElement<ModuleReference>(methodImport.Module));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Attributes" />
        public MethodImportAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Module" />
        public ModuleReference Module => _module.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Name" />
        public string Name => _name.Value;
    }
}
