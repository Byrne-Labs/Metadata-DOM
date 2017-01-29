using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class GenericContext
    {
        public GenericContext()
        {
            TypeParameters = ImmutableArray<TypeBase>.Empty;
            MethodParameters = ImmutableArray<TypeBase>.Empty;
        }

        public GenericContext(IEnumerable<TypeBase> typeParameters, IEnumerable<TypeBase> methodParameters)
        {
            TypeParameters = typeParameters == null ? ImmutableArray<TypeBase>.Empty : typeParameters.ToImmutableArray();
            MethodParameters = methodParameters == null ? ImmutableArray<TypeBase>.Empty : methodParameters.ToImmutableArray();
        }

        public bool ContextAvailable => MethodParameters.Any() || TypeParameters.Any();

        public ImmutableArray<TypeBase> MethodParameters { get; }

        public ImmutableArray<TypeBase> TypeParameters { get; }
    }
}
