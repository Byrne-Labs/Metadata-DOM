using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodReference : MethodReferenceBase, IMethod
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "The definition must be a method, not a constructor")]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal MethodReference(MemberReferenceHandle metadataHandle, MethodDefinition methodDefinition, MetadataState metadataState) : base(metadataHandle, methodDefinition, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal MethodReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, null, metadataState)
        {
        }

        public TypeBase ReturnType => MethodSignature?.ReturnType;
    }
}
