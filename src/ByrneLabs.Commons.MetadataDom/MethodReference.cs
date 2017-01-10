using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodReference : MethodReferenceBase, IMethod
    {
        internal MethodReference(MemberReferenceHandle metadataHandle, MethodDefinition methodDefinition, MetadataState metadataState) : base(metadataHandle, methodDefinition, metadataState)
        {
        }

        internal MethodReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, null, metadataState)
        {
        }

        public TypeBase ReturnType => MethodSignature?.ReturnType;
    }
}
