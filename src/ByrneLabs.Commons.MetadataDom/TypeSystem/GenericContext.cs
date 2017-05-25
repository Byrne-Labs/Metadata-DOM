using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal class GenericContext
    {
        public GenericContext(IManagedCodeElement requestingCodeElement)
        {
            RequestingCodeElement = requestingCodeElement;
            TypeParameters = ImmutableArray<Type>.Empty;
            MethodParameters = ImmutableArray<Type>.Empty;
        }

        public GenericContext(IManagedCodeElement requestingCodeElement, IEnumerable<Type> typeParameters, IEnumerable<Type> methodParameters)
        {
            RequestingCodeElement = requestingCodeElement;
            TypeParameters = typeParameters == null ? ImmutableArray<Type>.Empty : typeParameters.ToImmutableArray();
            MethodParameters = methodParameters == null ? ImmutableArray<Type>.Empty : methodParameters.ToImmutableArray();
        }

        public bool ContextAvailable => MethodParameters.Any() || TypeParameters.Any();

        public ImmutableArray<Type> MethodParameters { get; }

        public IManagedCodeElement RequestingCodeElement { get; }

        public ImmutableArray<Type> TypeParameters { get; }
    }
}
