using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition" />
    [PublicAPI]
    public class ModuleDefinition : RuntimeCodeElement, ICodeElementWithHandle<ModuleDefinitionHandle, System.Reflection.Metadata.ModuleDefinition>
    {
        internal ModuleDefinition(ModuleDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetModuleDefinition();
            Name = AsString(MetadataToken.Name);
            BaseGenerationId = AsGuid(MetadataToken.BaseGenerationId);
            Generation = MetadataToken.Generation;
            GenerationId = AsGuid(MetadataToken.GenerationId);
            Mvid = AsGuid(MetadataToken.Mvid);
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
        public string Name { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ModuleDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.ModuleDefinition MetadataToken { get; }
    }
}
