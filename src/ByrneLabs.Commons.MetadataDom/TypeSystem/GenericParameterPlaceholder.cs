using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using AssemblyToExpose = System.Reflection.Assembly;

#else
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class GenericParameterPlaceholder : EmptyTypeBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameterPlaceholder(GenericParameterPlaceholder baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<GenericParameterPlaceholder>(typeElementModifier))
        {
            Parent = baseType.Parent;
            Index = baseType.Index;
            UndecoratedName = $"T{Index}";
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameterPlaceholder(IManagedCodeElement parent, int index, MetadataState metadataState) : base(new CodeElementKey<GenericParameterPlaceholder>(parent, index), metadataState)
        {
            Parent = parent;
            Index = index;
            UndecoratedName = $"T{Index}";
        }

        public override AssemblyToExpose Assembly => MetadataState.AssemblyDefinition;

        public int Index { get; }

        public override bool IsGenericParameter => true;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => DeclaringType?.Namespace;

        internal IManagedCodeElement Parent { get; }

        internal override string UndecoratedName { get; }
    }
}
