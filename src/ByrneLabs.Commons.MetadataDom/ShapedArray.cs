using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ShapedArray : TypeBase<ShapedArray, Tuple<TypeBase, ArrayShape>>
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ShapedArray(ShapedArray baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ShapedArray(ShapedArray genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ShapedArray(TypeBase elementType, ArrayShape arrayShape, MetadataState metadataState) : base(new Tuple<TypeBase, ArrayShape>(elementType, arrayShape), metadataState)
        {
            BaseType = metadataState.GetCodeElement<SystemArray>();
            ElementType = elementType;
            ArrayRank = arrayShape.Rank;
            UndecoratedName = $"{elementType.Name}[{ new string(',', ArrayRank - 1) }]";
        }

        public override IAssembly Assembly { get; } = null;

        public override string AssemblyQualifiedName { get; } = null;

        internal override TypeBase BaseType { get; }

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType { get; } = null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.TypeInfo;

        public override int MetadataToken => 0;

        public override string Namespace => ElementType.Namespace;

        internal override string UndecoratedName { get; }
    }
}
