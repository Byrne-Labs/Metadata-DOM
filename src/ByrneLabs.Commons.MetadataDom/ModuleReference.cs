using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class ModuleReference : ModuleBase<ModuleReference, ModuleReferenceHandle, System.Reflection.Metadata.ModuleReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = AsString(MetadataToken.Name);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        public override IAssembly Assembly => null;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;
    }
}
