using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public sealed class SystemArray : EmptyTypeBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for SystemArray types")]
        internal SystemArray(SystemArray baseType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : base(baseType, typeElementModifier, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for SystemArray types")]
        internal SystemArray(SystemArray genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SystemArray(MetadataState metadataState) : base(new CodeElementKey<SystemArray>(), metadataState)
        {
        }

        public override string Name { get; } = "Array";

        public override string Namespace { get; } = "System";

        internal override string UndecoratedName { get; } = "System.Array";
    }
}
