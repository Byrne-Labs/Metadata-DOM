using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class SystemType : EmptyTypeBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for SystemType")]
        internal SystemType(SystemType baseType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : base(baseType, typeElementModifier, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for SystemType")]
        internal SystemType(SystemType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SystemType(MetadataState metadataState) : base(new CodeElementKey<SystemType>(), metadataState)
        {
        }

        public override string Namespace { get; } = "System";

        public override string TextSignature => FullName;

        internal override string UndecoratedName { get; } = "Type";

        protected override TypeAttributes GetAttributeFlagsImpl() => TypeAttributes.Class;
    }
}
