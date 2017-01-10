using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ConstructorReference : MethodReferenceBase, IConstructor
    {
        internal ConstructorReference(MemberReferenceHandle metadataHandle, ConstructorDefinition constructorDefinition, MetadataState metadataState) : base(metadataHandle, constructorDefinition, metadataState)
        {
        }

        internal ConstructorReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, null, metadataState)
        {
        }

    }
}
