using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MemberReference" />
    [PublicAPI]
    public class MemberReference : RuntimeCodeElement, ICodeElementWithHandle<MemberReferenceHandle, System.Reflection.Metadata.MemberReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _signature;

        internal MemberReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMemberReference(metadataHandle);
            Name = AsString(MetadataToken.Name);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            Kind = MetadataToken.GetKind();
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

        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MemberReferenceHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MemberReference MetadataToken { get; }
    }
}
