using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class ConstructorDefinition : MethodDefinitionBase, IConstructor
    {
        private readonly Lazy<string> _fullName;

        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}{Name}";
                var genericParameters = GenericTypeParameters.Any() ? $"<{ string.Join(", ", GenericTypeParameters.Select(genericTypeParameter => genericTypeParameter.Name)) }>" : string.Empty;
                var parameters = $"({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : parameter.ParameterType.FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
        }

        public override string FullName => _fullName.Value;

        public override string TextSignature => FullName;

        public override ImmutableArray<GenericParameter> GenericTypeParameters { get; } = ImmutableArray<GenericParameter>.Empty;

        public override bool IsGenericMethod { get; } = false;
    }
}
