using System;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeReference" />
    [PublicAPI]
    public class TypeReference : TypeBase, ICodeElementWithHandle<TypeReferenceHandle, System.Reflection.Metadata.TypeReference>
    {
        private readonly Lazy<CodeElement> _resolutionScope;

        internal TypeReference(TypeReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetTypeReference(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Namespace = AsString(MetadataToken.Namespace);
            _resolutionScope = new Lazy<CodeElement>(() => LoadResolutionScope(MetadataToken.ResolutionScope));
        }

        public override string FullName => (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".") + Name;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Name" />
        public override string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeDefinition.Namespace" />
        /// <summary>Full name of the namespace where the type is defined, or null if the type is nested or defined in a root namespace.</summary>
        public override string Namespace { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeReference.ResolutionScope" />
        /// <summary>Resolution scope in which the target type is defined and is uniquely identified by the specified
        ///     <see cref="Namespace" /> and <see cref="Name" />.</summary>
        /// <remarks>Resolution scope can be one of the following handles:
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="TypeReference" /> of the enclosing type, if the target type is a nested type.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="ModuleReference" />, if the target type is defined in another module within the same assembly as this one.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="ModuleDefinition" />, if the target type is defined in the current module. This should not occur in a CLI compressed metadata module.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="AssemblyReference" />, if the target type is defined in a different assembly from the current module.</description>
        ///         </item>
        ///         <item>
        ///             <description>Possibly a surprise if the handle was nil because that means it was resolved by searching the
        ///                 <see cref="ReflectionData.ExportedTypes" /> for a matching
        ///                 <see cref="Namespace" /> and <see cref="Name" />.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public CodeElement ResolutionScope => _resolutionScope.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public TypeReferenceHandle MetadataHandle { get; }

        public System.Reflection.Metadata.TypeReference MetadataToken { get; }

        private CodeElement LoadResolutionScope(EntityHandle resolutionScopeHandle) =>
            !resolutionScopeHandle.IsNil ?
                GetCodeElementWithHandle(resolutionScopeHandle) :
                MetadataState.ExportedTypes.SingleOrDefault(exportedType => exportedType.Name.Equals(Name) && exportedType.Namespace.Equals(Namespace));
    }
}
