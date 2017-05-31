using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class SystemValueType : EmptyTypeBase
    {
        internal SystemValueType(EmptyTypeBase unmodifiedType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : base(unmodifiedType, typeElementModifier, metadataState, key)
        {
        }

        internal SystemValueType(EmptyTypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        private SystemValueType(MetadataState metadataState) : base(new CodeElementKey<SystemValueType>(), metadataState)
        {
        }

        public override string Name => "ValueType";

        public override string Namespace => "System";

        internal override string UndecoratedName => "ValueType";
    }
}
