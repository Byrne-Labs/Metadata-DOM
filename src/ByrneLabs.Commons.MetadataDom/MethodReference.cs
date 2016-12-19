using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodReference : MethodReferenceBase, IMethod
    {
        internal MethodReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public override string FullName => $"{DeclaringType.FullName}.{Name}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";

        public override string TextSignature => $"{ReturnType.TextSignature} {FullName}";

        public TypeBase ReturnType => MethodSignature.Value.ReturnType;
    }
}
