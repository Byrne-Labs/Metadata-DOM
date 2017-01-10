using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public class SystemType : TypeBase
    {
        internal SystemType(TypeBase baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState, CodeElementKey key) : base(baseType, typeElementModifiers, metadataState, key)
        {
        }

        internal SystemType(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        internal SystemType(MetadataState metadataState) : base(new CodeElementKey<SystemType>(), metadataState)
        {
        }

        public override IAssembly Assembly { get; } = null;

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override string Name { get; } = "Type";

        public override string Namespace { get; } = "System";
    }
}
