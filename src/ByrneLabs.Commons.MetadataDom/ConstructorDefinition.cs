using System.Collections.Generic;
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

        public override string TextSignature => $"{DeclaringType.FullName}{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.TextSignature))})";

        public override IEnumerable<GenericParameter> GenericTypeParameters { get; } = Enumerable.Empty<GenericParameter>();

        public override bool IsGenericMethod { get; } = false;
    }
}
