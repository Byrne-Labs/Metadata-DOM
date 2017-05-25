using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class ConstructorReference : MethodReferenceBase
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "The definition must be a constructor, not a method")]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ConstructorReference(MemberReferenceHandle metadataHandle, ConstructorDefinition constructorDefinition, MetadataState metadataState) : base(metadataHandle, constructorDefinition, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ConstructorReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, null, metadataState)
        {
        }

        public override bool IsConstructor => true;
    }
}
