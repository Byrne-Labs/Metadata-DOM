using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public sealed class SystemArray : TypeBase
    {
        private SystemArray(TypeBase baseType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : base(baseType, typeElementModifier, metadataState, key)
        {
        }

        private SystemArray(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        private SystemArray(MetadataState metadataState) : base(new CodeElementKey<SystemArray>(), metadataState)
        {
        }

        public override IAssembly Assembly { get; } = null;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override string Name { get; } = "Array";

        public override string Namespace { get; } = "System";

        protected override string MetadataNamespace { get; } = null;

        internal override string UndecoratedName { get; } = "System.Array";
    }
}
