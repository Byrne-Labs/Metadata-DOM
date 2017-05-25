using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public abstract class MemberReferenceBase : IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<IManagedCodeElement> _parent;

        internal MemberReferenceBase(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            MetadataState = metadataState;
            MetadataHandle = (MemberReferenceHandle) key.UpcastHandle;
            RawMetadata = MetadataState.AssemblyReader.GetMemberReference(MetadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _parent = new Lazy<IManagedCodeElement>(() =>
            {
                IManagedCodeElement parent;
                if (RawMetadata.Parent.Kind == HandleKind.TypeSpecification)
                {
                    parent = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Parent, this);
                }
                else
                {
                    parent = MetadataState.GetCodeElement(RawMetadata.Parent);
                }

                return parent;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Kind = RawMetadata.GetKind();
        }

        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public MemberReferenceKind Kind { get; }

        public MemberReferenceHandle MetadataHandle { get; }

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        public string FullName => Name;

        public string Name { get; }

        public string TextSignature => Name;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
