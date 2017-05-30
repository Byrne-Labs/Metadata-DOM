using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class ModuleReference : ModuleBase<ModuleReference, ModuleReferenceHandle, System.Reflection.Metadata.ModuleReference>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override AssemblyToExpose Assembly => null;

        public override Guid BaseGenerationId => throw new NotSupportedException();

        public override int Generation => throw new NotSupportedException();

        public override Guid GenerationId => throw new NotSupportedException();

        public override bool Manifest { get; }

        public override Guid ModuleVersionId => throw new NotSupportedException();

        public override string Name { get; }

        public override string ScopeName { get; }

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override TypeToExpose[] GetTypes() => throw new NotSupportedException();

        public override bool IsResource() => throw new NotSupportedException();

        protected override FieldInfoToExpose[] GetAllFields() => throw new NotSupportedException();

        protected override MethodInfoToExpose[] GetAllMethods() => throw new NotSupportedException();
    }
}
