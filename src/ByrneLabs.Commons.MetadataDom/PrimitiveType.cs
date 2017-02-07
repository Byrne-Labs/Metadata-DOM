using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class PrimitiveType : TypeBase<PrimitiveType, PrimitiveTypeCode>
    {
        private Lazy<int> _metadataToken;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveType baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveTypeCode primitiveTypeCode, MetadataState metadataState) : base(primitiveTypeCode, metadataState)
        {
            Initialize();
        }

        public override IAssembly Assembly { get; } = null;

        public override string AssemblyQualifiedName { get; } = null;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override int MetadataToken => _metadataToken.Value;

        public override string Namespace { get; } = "System";

        public PrimitiveTypeCode PrimitiveTypeCode => KeyValue;

        protected override string MetadataNamespace { get; } = null;

        internal override string UndecoratedName => PrimitiveTypeCode.ToString();

        private void Initialize()
        {
            _metadataToken = new Lazy<int>(() => Type.GetType($"System.{PrimitiveTypeCode}").GetTypeInfo().MetadataToken);
        }
    }
}
