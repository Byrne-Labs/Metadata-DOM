using System;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class MethodImport : CodeElementWithoutHandle
    {
        private readonly Lazy<ModuleReference> _module;
        private readonly Lazy<string> _name;

        internal MethodImport(System.Reflection.Metadata.MethodImport methodImport, MetadataState metadataState) : base(methodImport, metadataState)
        {
            Attributes = methodImport.Attributes;
            _name = new Lazy<string>(() => AsString(methodImport.Name));
            _module = new Lazy<ModuleReference>(() => GetCodeElement<ModuleReference>(methodImport.Module));
        }

        public MethodImportAttributes Attributes { get; }

        public ModuleReference Module => _module.Value;

        public string Name => _name.Value;
    }
}
