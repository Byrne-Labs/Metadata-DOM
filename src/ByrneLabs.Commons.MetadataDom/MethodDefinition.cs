using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class MethodDefinition : MethodDefinitionBase, IMethod
    {
        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public PropertyDefinition RelatedProperty { get; internal set; }

        public override string TextSignature => FullName + (IsSpecialName ? string.Empty : $"({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.TextSignature))})");

        /// <summary>Returns <see cref="TypeDefinition" />, <see cref="TypeReference" />, <see cref="TypeSpecification" />, <see cref="GenericParameter" />, or null when void</summary>
        public TypeBase ReturnType => Signature.ReturnType;
    }
}
