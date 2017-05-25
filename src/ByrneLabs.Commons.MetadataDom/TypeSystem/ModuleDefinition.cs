using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using AssemblyToExpose = System.Reflection.Assembly;
using FieldInfoToExpose = System.Reflection.FieldInfo;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
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

        public override AssemblyToExpose Assembly => MetadataState.AssemblyDefinition;

        public override Guid BaseGenerationId { get; }

        public override int Generation { get; }

        public override Guid GenerationId { get; }

        public override bool Manifest { get; }

        public override Guid ModuleVersionId { get; }

        public override string Name { get; }

        public override string ScopeName { get; }

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => Enumerable.Empty<CustomAttributeDataToExpose>().ToImmutableList();

        public override TypeToExpose[] GetTypes() => throw new NotSupportedException();

        public override bool IsResource() => throw new NotSupportedException();

        protected override FieldInfoToExpose[] GetAllFields() => throw new NotSupportedException();

        protected override MethodInfoToExpose[] GetAllMethods() => throw new NotSupportedException();
    }
}
