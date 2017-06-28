using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class ModuleReference : ModuleBase<ModuleReference, ModuleReferenceHandle, System.Reflection.Metadata.ModuleReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override System.Reflection.Assembly Assembly => null;

        public override Guid BaseGenerationId => throw NotSupportedHelper.FutureVersion();

        public override int Generation => throw NotSupportedHelper.FutureVersion();

        public override Guid GenerationId => throw NotSupportedHelper.FutureVersion();

        public override bool Manifest { get; }

        public override Guid ModuleVersionId => throw NotSupportedHelper.FutureVersion();

        public override string Name { get; }

        public override string ScopeName { get; }

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetTypes() => throw NotSupportedHelper.FutureVersion();

        public override bool IsResource() => throw NotSupportedHelper.FutureVersion();

        protected override System.Reflection.FieldInfo[] GetAllFields() => throw NotSupportedHelper.FutureVersion();

        protected override System.Reflection.MethodInfo[] GetAllMethods() => throw NotSupportedHelper.FutureVersion();
    }
}
