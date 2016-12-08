using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MemberReference" />
    [PublicAPI]
    public class MemberReference : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _signature;

        internal MemberReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var memberResource = Reader.GetMemberReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(memberResource.Name));
            _parent = new Lazy<CodeElement>(() => GetCodeElement(memberResource.Parent));
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(memberResource.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(memberResource.GetCustomAttributes()));
            Kind = memberResource.GetKind();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetKind" />
        public MemberReferenceKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Parent" />
        /// <summary>
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.MethodDefinition" />, <see cref="T:ByrneLabs.Commons.MetadataDom.ModuleReference" />,
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeDefinition" />, <see cref="T:ByrneLabs.Commons.MetadataDom.TypeReference" />, or
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeSpecification" />.</summary>
        public CodeElement Parent => _parent.Value;

        public Blob Signature => _signature.Value;
    }
}
