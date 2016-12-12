using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class PrimitiveType : TypeBase<PrimitiveType, PrimitiveTypeCode>
    {
        internal PrimitiveType(PrimitiveType baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal PrimitiveType(PrimitiveType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal PrimitiveType(PrimitiveTypeCode primitiveTypeCode, MetadataState metadataState) : base(primitiveTypeCode, metadataState)
        {
            Initialize();
        }

        public PrimitiveTypeCode PrimitiveTypeCode => KeyValue;

        private void Initialize()
        {
            Name = PrimitiveTypeCode.ToString();
            FullName = $"System.{Name}";
            Namespace = "System";
        }
    }
}
