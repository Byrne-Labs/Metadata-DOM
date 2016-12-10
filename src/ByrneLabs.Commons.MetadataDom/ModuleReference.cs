using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference" />
    [PublicAPI]
    public class ModuleReference : RuntimeCodeElement, ICodeElementWithHandle<ModuleReferenceHandle, System.Reflection.Metadata.ModuleReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetModuleReference(metadataHandle);
            Name = AsString(MetadataToken.Name);

            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference.Name" />
        public string Name { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ModuleReferenceHandle MetadataHandle { get; }

        public System.Reflection.Metadata.ModuleReference MetadataToken { get; }
    }
}
