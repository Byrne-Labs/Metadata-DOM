using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created using reflection")]
    public class ShapedArray : EmptyTypeBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This constructor is only valid for ShapedArrays")]
        internal ShapedArray(ShapedArray baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<ShapedArray>(baseType, typeElementModifier))
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
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This constructor is only valid for TypeBase")]
        internal ShapedArray(TypeBase elementType, ArrayShape arrayShape, MetadataState metadataState) : base(new CodeElementKey<ShapedArray>(elementType, arrayShape), metadataState)
        {
            BaseType = metadataState.GetCodeElement<SystemArray>();
            ElementType = elementType;
            ArrayRank = arrayShape.Rank;
            UndecoratedName = $"{elementType.Name}[{new string(',', ArrayRank - 1)}]";
        }

        public sealed override int ArrayRank { get; }

        public override Type BaseType { get; }

        public override TypeInfo ElementType { get; }

        public override int MetadataToken => 0;

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Namespace => ElementType.Namespace;

        internal override string UndecoratedName { get; }

        protected override TypeAttributes GetAttributeFlagsImpl() => ElementType.Attributes;
    }
}
