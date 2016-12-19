using System;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class MethodDefinition : MethodDefinitionBase, IMethod
    {
        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override string TextSignature => $"{ReturnType.TextSignature} {FullName}({string.Join(", ", Parameters.Select(parameter => parameter.TextSignature))})";

        /// <summary>Returns <see cref="TypeDefinition" />, <see cref="TypeReference" />, <see cref="TypeSpecification" />, <see cref="GenericParameter" />, or null when void</summary>
        public TypeBase ReturnType => Signature.ReturnType;
    }
}
