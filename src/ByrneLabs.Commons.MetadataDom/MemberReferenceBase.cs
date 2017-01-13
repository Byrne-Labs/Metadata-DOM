using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MemberReference" />
    //[PublicAPI]
    public abstract class MemberReferenceBase : RuntimeCodeElement, ICodeElementWithTypedHandle<MemberReferenceHandle, MemberReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _parent;

        internal MemberReferenceBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            MetadataHandle = (MemberReferenceHandle) key.UpcastHandle;
            RawMetadata = Reader.GetMemberReference(MetadataHandle);
            Name = AsString(RawMetadata.Name);
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Kind = RawMetadata.GetKind();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetKind" />
        public MemberReferenceKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Parent" />
        /// <summary><see cref="MethodDefinition" />, <see cref="ModuleReference" />, <see cref="TypeDefinition" />, <see cref="TypeReference" />, or
        ///     <see cref="TypeSpecification" />.</summary>
        public CodeElement Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public MemberReferenceHandle MetadataHandle { get; }
    }
}
