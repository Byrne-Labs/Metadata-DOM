using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class PrimitiveType : EmptyTypeBase
    {
        private readonly Lazy<int> _metadataToken;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveType baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<PrimitiveType>(baseType, typeElementModifier))
        {
            PrimitiveTypeCode = baseType.PrimitiveTypeCode;
            _metadataToken = new Lazy<int>(() => System.Type.GetType($"System.{PrimitiveTypeCode}").GetTypeInfo().MetadataToken);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<PrimitiveType>(genericTypeDefinition, genericTypeArguments))
        {
            PrimitiveTypeCode = genericTypeDefinition.PrimitiveTypeCode;
            _metadataToken = new Lazy<int>(() => System.Type.GetType($"System.{PrimitiveTypeCode}").GetTypeInfo().MetadataToken);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal PrimitiveType(PrimitiveTypeCode primitiveTypeCode, MetadataState metadataState) : base(new CodeElementKey<PrimitiveType>(primitiveTypeCode), metadataState)
        {
            PrimitiveTypeCode = primitiveTypeCode;
            _metadataToken = new Lazy<int>(() => System.Type.GetType($"System.{PrimitiveTypeCode}").GetTypeInfo().MetadataToken);
        }

        public override int MetadataToken => _metadataToken.Value;

        public override string Namespace { get; } = "System";

        public PrimitiveTypeCode PrimitiveTypeCode { get; }

        internal override string MetadataNamespace { get; } = "System";

        internal override string UndecoratedName => PrimitiveTypeCode.ToString();

        protected override TypeAttributes GetAttributeFlagsImpl() => TypeAttributes.AnsiClass & TypeAttributes.Class;
    }
}
