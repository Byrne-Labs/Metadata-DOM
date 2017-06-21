using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class ModuleDefinition : ModuleBase<ModuleDefinition, ModuleDefinitionHandle, System.Reflection.Metadata.ModuleDefinition>
    {
        internal ModuleDefinition(ModuleDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            BaseGenerationId = MetadataState.AssemblyReader.GetGuid(RawMetadata.BaseGenerationId);
            Generation = RawMetadata.Generation;
            GenerationId = MetadataState.AssemblyReader.GetGuid(RawMetadata.GenerationId);
            ModuleVersionId = MetadataState.AssemblyReader.GetGuid(RawMetadata.Mvid);
        }

        public override System.Reflection.Assembly Assembly => MetadataState.AssemblyDefinition;

        public override Guid BaseGenerationId { get; }

        public override int Generation { get; }

        public override Guid GenerationId { get; }

        public override bool Manifest { get; }

        public override Guid ModuleVersionId { get; }

        public override string Name { get; }

        public override string ScopeName { get; }

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => Enumerable.Empty<System.Reflection.CustomAttributeData>().ToImmutableList();

        public override Type[] GetTypes() => throw NotSupportedHelper.FutureVersion();

        public override bool IsResource() => throw NotSupportedHelper.FutureVersion();

        protected override System.Reflection.FieldInfo[] GetAllFields() => throw NotSupportedHelper.FutureVersion();

        protected override System.Reflection.MethodInfo[] GetAllMethods() => throw NotSupportedHelper.FutureVersion();
    }
}
