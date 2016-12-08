using System;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeReference" />
    [PublicAPI]
    public class TypeReference : CodeElementWithHandle
    {
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<CodeElement> _resolutionScope;

        internal TypeReference(TypeReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var typeReference = Reader.GetTypeReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(typeReference.Name));
            _namespace = new Lazy<string>(() => AsString(typeReference.Namespace));
            _resolutionScope = new Lazy<CodeElement>(() => LoadResolutionScope(typeReference.ResolutionScope));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeReference.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeReference.Namespace" />
        public string Namespace => _namespace.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeReference.ResolutionScope" />
        /// <summary>Resolution scope in which the target type is defined and is uniquely identified by the specified
        ///     <see cref="P:ByrneLabs.Commons.MetadataDom.TypeReference.Namespace" /> and <see cref="P:System.Reflection.Metadata.TypeReference.Name" />.</summary>
        /// <remarks>Resolution scope can be one of the following handles:
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="T:ByrneLabs.Commons.MetadataDom.TypeReference" /> of the enclosing type, if the target type is a nested type.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="T:ByrneLabs.Commons.MetadataDom.ModuleReference" />, if the target type is defined in another module within the same assembly as this one.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="F:ByrneLabs.Commons.MetadataDom.ModuleDefinition" />, if the target type is defined in the current module. This should not occur in a CLI compressed metadata module.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="T:ByrneLabs.Commons.MetadataDom.AssemblyReferenceHandle" />, if the target type is defined in a different assembly from the current module.</description>
        ///         </item>
        ///         <item>
        ///             <description>Possibly a surprise if the handle was nil because that means it was resolved by searching the
        ///                 <see cref="P:System.Reflection.Metadata.MetadataReader.ExportedTypes" /> for a matching
        ///                 <see cref="P:ByrneLabs.Commons.MetadataDom.TypeReference.Namespace" /> and <see cref="P:ByrneLabs.Commons.MetadataDom.TypeReference.Name" />.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public CodeElement ResolutionScope => _resolutionScope.Value;

        private CodeElement LoadResolutionScope(EntityHandle resolutionScopeHandle) =>
            !resolutionScopeHandle.IsNil ?
                GetCodeElement(resolutionScopeHandle) :
                MetadataState.ExportedTypes.SingleOrDefault(exportedType => exportedType.Name.Equals(Name) && exportedType.Namespace.Equals(Namespace));
    }
}
