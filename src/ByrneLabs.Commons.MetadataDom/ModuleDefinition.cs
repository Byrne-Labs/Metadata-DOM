using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition" />
    //[PublicAPI]
    public class ModuleDefinition : ModuleBase<ModuleDefinition, ModuleDefinitionHandle, System.Reflection.Metadata.ModuleDefinition>
    {
        internal ModuleDefinition(ModuleDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = AsString(MetadataToken.Name);
            BaseGenerationId = AsGuid(MetadataToken.BaseGenerationId);
            Generation = MetadataToken.Generation;
            GenerationId = AsGuid(MetadataToken.GenerationId);
            Mvid = AsGuid(MetadataToken.Mvid);
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.BaseGenerationId" />
        public Guid BaseGenerationId { get; }

        public override IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Generation" />
        public int Generation { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.GenerationId" />
        public Guid GenerationId { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Mvid" />
        public Guid Mvid { get; }
    }
}
