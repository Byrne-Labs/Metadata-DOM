using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class MethodDefinition : MethodDefinitionBase, IMethod
    {
        private readonly Lazy<ImmutableArray<GenericParameter>> _genericParameters;

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
        }

        public override ImmutableArray<GenericParameter> GenericTypeParameters => _genericParameters.Value;

        public PropertyDefinition RelatedProperty { get; internal set; }

        public override string TextSignature => $"{DeclaringType.FullName}.{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.FullNameWithoutAssemblies))})";

        /// <summary>Returns <see cref="TypeDefinition" />, <see cref="TypeReference" />, <see cref="TypeSpecification" />, <see cref="GenericParameter" />, or null when void</summary>
        public TypeBase ReturnType => Signature.ReturnType;

        public override bool IsGenericMethod => GenericTypeParameters.Any();
    }
}
