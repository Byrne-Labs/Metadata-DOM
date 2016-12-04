using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MemberReference : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _signature;

        internal MemberReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var memberResource = Reader.GetMemberReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(memberResource.Name));
            _parent = new Lazy<CodeElement>(() => GetCodeElement(memberResource.Parent));
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(memberResource.Signature)));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(memberResource.GetCustomAttributes()));
            Kind = memberResource.GetKind();
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public MemberReferenceKind Kind { get; }

        public string Name => _name.Value;

        public CodeElement Parent => _parent.Value;

        public Blob Signature => _signature.Value;
    }
}
