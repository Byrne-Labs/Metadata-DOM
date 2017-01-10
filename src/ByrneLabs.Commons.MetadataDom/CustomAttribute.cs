using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Constructor.DeclaringType.FullName}")]
    public class CustomAttribute : RuntimeCodeElement, ICodeElementWithHandle<CustomAttributeHandle, System.Reflection.Metadata.CustomAttribute>
    {
        private readonly Lazy<IConstructor> _constructor;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<CustomAttributeValue<TypeBase>?> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetCustomAttribute(metadataHandle);
            _constructor = MetadataState.GetLazyCodeElement<IConstructor>(MetadataToken.Constructor, null);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _value = new Lazy<CustomAttributeValue<TypeBase>?>(() =>
            {
                CustomAttributeValue<TypeBase>? value;

                /*
                 * I cannot figure out why, but the DecodeValue call will throw an exception if the argument kind is not field or property. -- Jonathan Byrne 12/19/2016
                 */
                var valueKind = (CustomAttributeNamedArgumentKind)Reader.GetBlobReader(MetadataToken.Value).ReadSerializationTypeCode();
                if (valueKind == CustomAttributeNamedArgumentKind.Field || valueKind == CustomAttributeNamedArgumentKind.Property)
                {
                    value = MetadataToken.DecodeValue(MetadataState.TypeProvider);
                }
                else
                {
                    value = null;
                }

                return value;
            });
        }

        public TypeBase AttributeType => Constructor.DeclaringType;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Constructor" />
        /// <summary>The constructor (<see cref="MethodDefinition" /> or <see cref="MemberReferenceBase" />) of the custom attribute type.</summary>
        public IConstructor Constructor => _constructor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Value" />
        public CustomAttributeValue<TypeBase>? Value => _value.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public CustomAttributeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.CustomAttribute MetadataToken { get; }
    }
}
