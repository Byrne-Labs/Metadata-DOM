using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public sealed class SystemArray : TypeBase
    {
        private SystemArray(TypeBase baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState, CodeElementKey key) : base(baseType, typeElementModifiers, metadataState, key)
        {
        }

        private SystemArray(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        private SystemArray(MetadataState metadataState) : base(new CodeElementKey<SystemArray>(), metadataState)
        {
        }

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType { get; } = null;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override IAssembly Assembly { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override string Namespace { get; } = "System";

        public override string Name { get; } = "Array";

        internal override string UndecoratedName { get; } = "System.Array";
    }
}
