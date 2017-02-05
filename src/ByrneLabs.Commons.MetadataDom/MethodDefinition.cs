using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class MethodDefinition : MethodDefinitionBase, IMethod
    {
        private readonly Lazy<ImmutableArray<GenericParameter>> _genericParameters;
        private readonly Lazy<string> _fullName;

        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _genericParameters = new Lazy<ImmutableArray<GenericParameter>>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                var index = 0;
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.DeclaringMethod = this;
                    genericParameter.SetDeclaringType(DeclaringType);
                    genericParameter.Index = index++;
                }

                return genericParameters;
            });
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}.{Name}";
                var genericParameters = GenericTypeParameters.Any() ? $"<{ string.Join(", ", GenericTypeParameters.Select(genericTypeParameter => genericTypeParameter.Name)) }>" : string.Empty;
                var parameters = IsSpecialName && RelatedProperty?.IsIndexer != true ? string.Empty : $"({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : parameter.ParameterType.FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
        }

        public PropertyDefinition RelatedProperty { get; internal set; }

        public EventDefinition RelatedEvent { get; internal set; }

        public override string FullName => _fullName.Value;

        public override string TextSignature => FullName;

        /// <summary>Returns <see cref="TypeDefinition" />, <see cref="TypeReference" />, <see cref="TypeSpecification" />, <see cref="GenericParameter" />, or null when void</summary>
        public TypeBase ReturnType => Signature.ReturnType;

        public override ImmutableArray<GenericParameter> GenericTypeParameters => _genericParameters.Value;

        public override bool IsGenericMethod => GenericTypeParameters.Any();
    }
}
