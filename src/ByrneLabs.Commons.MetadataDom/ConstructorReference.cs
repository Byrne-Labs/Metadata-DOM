using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ConstructorReference : MethodReferenceBase, IConstructor
    {
        internal ConstructorReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public override string FullName => $"{DeclaringType.FullName}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";

        public override string TextSignature => $"{DeclaringType.TextSignature}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";
    }
}
