using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class ConstructorDefinition : MethodDefinitionBase, IConstructor
    {
        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public override string TextSignature => $"{DeclaringType.FullName}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.TextSignature))})";
    }
}
