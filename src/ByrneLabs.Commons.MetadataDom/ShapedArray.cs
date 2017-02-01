﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created using reflection")]
    public class ShapedArray : TypeBase
    {

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This constructor is only valid for ShapedArrays")]
        internal ShapedArray(ShapedArray baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState, new CodeElementKey<ShapedArray>(baseType, typeElementModifiers))
        {
            BaseType = metadataState.GetCodeElement<SystemArray>();
            ElementType = baseType.ElementType;
            ArrayRank = baseType.ArrayRank;
            UndecoratedName = baseType.UndecoratedName;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This constructor is only valid for ShapedArrays")]
        internal ShapedArray(ShapedArray genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<ShapedArray>(genericTypeDefinition, genericTypeArguments))
        {
            BaseType = metadataState.GetCodeElement<SystemArray>();
            ElementType = genericTypeDefinition.ElementType;
            ArrayRank = genericTypeDefinition.ArrayRank;
            UndecoratedName = genericTypeDefinition.UndecoratedName;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal ShapedArray(TypeBase elementType, ArrayShape arrayShape, MetadataState metadataState) : base(new CodeElementKey<ShapedArray>(elementType, arrayShape), metadataState)
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
