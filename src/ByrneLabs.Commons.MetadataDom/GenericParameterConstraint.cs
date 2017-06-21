using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class GenericParameterConstraint
    {
        public abstract IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get; }

        public abstract Type Parameter { get; }

        public abstract Type Type { get; }

        public override string ToString() => Parameter.Name;
    }
}
