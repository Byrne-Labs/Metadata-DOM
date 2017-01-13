using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class PrimitiveType : TypeBase<PrimitiveType, PrimitiveTypeCode>
    {

        private Lazy<int> _metadataToken;

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

        public override IAssembly Assembly { get; } = null;

        public override string AssemblyQualifiedName { get; } = null;

        public override IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override int MetadataToken => _metadataToken.Value;

        public override string Namespace { get; } = "System";

        public PrimitiveTypeCode PrimitiveTypeCode => KeyValue;

        internal override string UndecoratedName => PrimitiveTypeCode.ToString();

        private void Initialize()
        {
            _metadataToken = new Lazy<int>(() => Type.GetType(FullName).GetTypeInfo().MetadataToken);
        }

    }
}
