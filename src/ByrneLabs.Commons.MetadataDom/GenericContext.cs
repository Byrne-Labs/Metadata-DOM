using System.Collections.Generic;
using System.Collections.Immutable;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class GenericContext
    {
        public GenericContext(IEnumerable<TypeBase> typeParameters, IEnumerable<TypeBase> methodParameters)
        {
            TypeParameters = typeParameters == null ? ImmutableArray<TypeBase>.Empty : typeParameters.ToImmutableArray();
            MethodParameters = methodParameters == null ? ImmutableArray<TypeBase>.Empty : methodParameters.ToImmutableArray();
        }

        public ImmutableArray<TypeBase> MethodParameters { get; }

        public ImmutableArray<TypeBase> TypeParameters { get; }
    }
}
