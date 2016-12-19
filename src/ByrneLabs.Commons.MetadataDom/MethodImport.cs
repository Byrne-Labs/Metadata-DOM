using System;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodImport" />
    //[PublicAPI]
    public class MethodImport : RuntimeCodeElement, ICodeElementWithToken<System.Reflection.Metadata.MethodImport>
    {
        private readonly Lazy<ModuleReference> _module;

        internal MethodImport(System.Reflection.Metadata.MethodImport methodImport, MetadataState metadataState) : base(new CodeElementKey<MethodImport>(methodImport), metadataState)
        {
            MetadataToken = methodImport;
            Attributes = methodImport.Attributes;
            Name = AsString(methodImport.Name);
            _module = MetadataState.GetLazyCodeElement<ModuleReference>(methodImport.Module);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Attributes" />
        public MethodImportAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Module" />
        public ModuleReference Module => _module.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImport.Name" />
        public string Name { get; }

        public System.Reflection.Metadata.MethodImport MetadataToken { get; }
    }
}
