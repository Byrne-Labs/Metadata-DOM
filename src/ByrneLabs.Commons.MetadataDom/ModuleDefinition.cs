using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ModuleDefinition : CodeElementWithHandle
    {
        private readonly Lazy<string> _name;

        internal ModuleDefinition(ModuleDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var moduleDefinition = Reader.GetModuleDefinition();
            _name = new Lazy<string>(() => AsString(moduleDefinition.Name));
            BaseGenerationId = AsGuid(moduleDefinition.BaseGenerationId);
            Generation = moduleDefinition.Generation;
            GenerationId = AsGuid(moduleDefinition.GenerationId);
            Mvid = AsGuid(moduleDefinition.Mvid);
        }

        public Guid BaseGenerationId { get; }

        public int Generation { get; }

        public Guid GenerationId { get; }

        public Guid Mvid { get; }

        public string Name => _name.Value;
    }
}
