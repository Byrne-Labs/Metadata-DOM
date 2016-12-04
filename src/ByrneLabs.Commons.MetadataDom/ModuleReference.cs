using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ModuleReference : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;

        internal ModuleReference(ModuleReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var moduleReference = Reader.GetModuleReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(moduleReference.Name));

            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(moduleReference.GetCustomAttributes()));
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public string Name => _name.Value;
    }
}
