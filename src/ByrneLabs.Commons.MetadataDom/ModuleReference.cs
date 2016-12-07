using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference" />
    public class ModuleReference : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var moduleReference = Reader.GetModuleReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(moduleReference.Name));

            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(moduleReference.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ModuleReference.Name" />
        public string Name => _name.Value;
    }
}
