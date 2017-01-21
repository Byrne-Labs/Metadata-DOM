using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition" />
    //[PublicAPI]
    public class ModuleDefinition : ModuleBase<ModuleDefinition, ModuleDefinitionHandle, System.Reflection.Metadata.ModuleDefinition>
    {
        internal ModuleDefinition(ModuleDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            ScopedName = AsString(RawMetadata.Name);
            BaseGenerationId = AsGuid(RawMetadata.BaseGenerationId);
            Generation = RawMetadata.Generation;
            GenerationId = AsGuid(RawMetadata.GenerationId);
            Mvid = AsGuid(RawMetadata.Mvid);
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.BaseGenerationId" />
        public Guid BaseGenerationId { get; }

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Generation" />
        public int Generation { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.GenerationId" />
        public Guid GenerationId { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleDefinition.Mvid" />
        public Guid Mvid { get; }
    }
}
