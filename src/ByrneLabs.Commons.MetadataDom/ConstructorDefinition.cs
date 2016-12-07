using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ConstructorDefinition : MethodDefinitionBase
    {
        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }
    }
}
