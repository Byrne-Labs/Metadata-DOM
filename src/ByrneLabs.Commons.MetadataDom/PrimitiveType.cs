using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class PrimitiveType : TypeBase<PrimitiveType, PrimitiveTypeCode>
    {
        internal PrimitiveType(PrimitiveType baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
        }

        internal PrimitiveType(PrimitiveType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
        }

        internal PrimitiveType(PrimitiveTypeCode primitiveTypeCode, MetadataState metadataState) : base(primitiveTypeCode, metadataState)
        {
        }

        public override IAssembly Assembly { get; } = null;

        public override string AssemblyQualifiedName { get; } = null;

        public IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override string Name => PrimitiveTypeCode.ToString();

        public override string Namespace { get; } = "System";

        public PrimitiveTypeCode PrimitiveTypeCode => KeyValue;
    }
}
