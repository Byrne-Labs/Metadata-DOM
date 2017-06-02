using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public abstract class Namespace
    {
        public abstract IEnumerable<Type> ExportedTypes { get; }

        public abstract string Name { get; }

        public abstract IEnumerable<Namespace> Namespaces { get; }

        public abstract Namespace Parent { get; }

        public abstract IEnumerable<Type> TypeDefinitions { get; }

        public override string ToString() => $"({GetType().FullName}) {Name}";
    }
}
