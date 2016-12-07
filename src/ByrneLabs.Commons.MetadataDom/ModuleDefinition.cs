using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.BaseGenerationId" />
        public Guid BaseGenerationId { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Generation" />
        public int Generation { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.GenerationId" />
        public Guid GenerationId { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Mvid" />
        public Guid Mvid { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Name" />
        public string Name => _name.Value;
    }
}
