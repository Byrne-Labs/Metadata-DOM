using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class GenericParameterPlaceholder : TypeBase<GenericParameterPlaceholder, object, object>
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameterPlaceholder(GenericParameterPlaceholder baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Parent = baseType.Parent;
            Index = baseType.Index;
            UndecoratedName = $"T{Index}";
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameterPlaceholder(CodeElement parent, int index, MetadataState metadataState) : base(new CodeElementKey<GenericParameterPlaceholder>(parent, index), metadataState)
        {
            Parent = parent;
            Index = index;
            UndecoratedName = $"T{Index}";
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        public GenericParameterAttributes Attributes { get; } = GenericParameterAttributes.None;

        public ImmutableArray<GenericParameterConstraint> Constraints { get; } = ImmutableArray<GenericParameterConstraint>.Empty;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public MethodDefinition DeclaringMethod => null;

        public override TypeBase DeclaringType => null;

        public override string FullName { get; } = null;

        public override string FullNameWithoutAssemblies { get; } = null;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Index" />
        public int Index { get; }

        public override bool IsGenericParameter { get; } = true;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => DeclaringType?.Namespace;

        public CodeElement Parent { get; }

        protected override string MetadataNamespace { get; } = null;

        internal override string FullNameWithoutGenericArguments { get; } = null;

        internal override string UndecoratedName { get; }
    }
}
