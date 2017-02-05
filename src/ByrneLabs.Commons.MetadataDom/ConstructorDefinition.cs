using System.Collections.Immutable;
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

        public override string FullName => $"{DeclaringType.FullName}{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.FullName))})";

        public override string TextSignature => $"{DeclaringType.FullName}{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.TextSignature))})";

        public override ImmutableArray<GenericParameter> GenericTypeParameters { get; } = ImmutableArray<GenericParameter>.Empty;

        public override bool IsGenericMethod { get; } = false;
    }
}
