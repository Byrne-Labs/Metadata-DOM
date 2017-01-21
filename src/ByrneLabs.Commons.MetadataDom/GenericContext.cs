using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class GenericContext
    {
        public GenericContext(IEnumerable<TypeBase> typeParameters, IEnumerable<TypeBase> methodParameters)
        {
            TypeParameters = typeParameters == null ? Enumerable.Empty<TypeBase>().ToImmutableArray() : typeParameters.ToImmutableArray();
            MethodParameters = methodParameters == null ? Enumerable.Empty<TypeBase>().ToImmutableArray() : methodParameters.ToImmutableArray();
        }

        public ImmutableArray<TypeBase> MethodParameters { get; }

        public ImmutableArray<TypeBase> TypeParameters { get; }
    }
}
