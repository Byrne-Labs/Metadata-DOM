using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Constructor.DeclaringType.FullName}")]
    public class CustomAttribute : RuntimeCodeElement, ICodeElementWithHandle<CustomAttributeHandle, System.Reflection.Metadata.CustomAttribute>
    {
        private readonly Lazy<IConstructor> _constructor;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<CustomAttributeValue<TypeBase>> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetCustomAttribute(metadataHandle);
            _constructor = MetadataState.GetLazyCodeElement<IConstructor>(MetadataToken.Constructor);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _value = new Lazy<CustomAttributeValue<TypeBase>>(() => MetadataToken.DecodeValue(MetadataState.TypeProvider));
        }

        public TypeBase AttributeType => Constructor.DeclaringType;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Constructor" />
        /// <summary>The constructor (<see cref="MethodDefinition" /> or <see cref="MemberReferenceBase" />) of the custom attribute type.</summary>
        public IConstructor Constructor => _constructor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Value" />
        public CustomAttributeValue<TypeBase> Value => _value.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public CustomAttributeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.CustomAttribute MetadataToken { get; }
    }
}
