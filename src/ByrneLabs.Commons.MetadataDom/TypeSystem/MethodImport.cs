using System;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class MethodImport : RuntimeCodeElement, ICodeElementWithRawMetadata<System.Reflection.Metadata.MethodImport>
    {
        private readonly Lazy<ModuleReference> _module;

        internal MethodImport(System.Reflection.Metadata.MethodImport methodImport, MetadataState metadataState) : base(new CodeElementKey<MethodImport>(methodImport), metadataState)
        {
            RawMetadata = methodImport;
            Attributes = methodImport.Attributes;
            Name = AsString(methodImport.Name);
            _module = MetadataState.GetLazyCodeElement<ModuleReference>(methodImport.Module);
        }
        public MethodImportAttributes Attributes { get; }
        public ModuleReference Module => _module.Value;
        public string Name { get; }

        public System.Reflection.Metadata.MethodImport RawMetadata { get; }
    }
}
