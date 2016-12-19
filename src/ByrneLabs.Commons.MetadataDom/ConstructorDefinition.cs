using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class ConstructorDefinition : MethodDefinitionBase, IConstructor
    {
        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public override string FullName => $"{DeclaringType.FullName}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";

        public override string TextSignature => $"{DeclaringType.TextSignature}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";
    }
}
