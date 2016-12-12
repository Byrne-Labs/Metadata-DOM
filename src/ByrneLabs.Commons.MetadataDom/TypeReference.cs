using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeReference" />
    [PublicAPI]
    public class TypeReference : TypeBase<TypeReference, TypeReferenceHandle, System.Reflection.Metadata.TypeReference>
    {
        private Lazy<CodeElement> _resolutionScope;

        internal TypeReference(TypeReference baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal TypeReference(TypeReference genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal TypeReference(TypeReferenceHandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            Initialize();
        }

        private void Initialize()
        {
            Name = AsString(MetadataToken.Name);
            Namespace = AsString(MetadataToken.Namespace);
            FullName = (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".") + Name;
            _resolutionScope = new Lazy<CodeElement>(() => LoadResolutionScope(MetadataToken.ResolutionScope));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeReference.ResolutionScope" />
        /// <summary>Resolution scope in which the target type is defined and is uniquely identified by the specified
        ///     <see cref="TypeBase.Namespace" /> and <see cref="TypeBase.Name" />.</summary>
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
        ///                 <see cref="TypeBase.Namespace" /> and <see cref="TypeBase.Name" />.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public CodeElement ResolutionScope => _resolutionScope.Value;

        private CodeElement LoadResolutionScope(EntityHandle resolutionScopeHandle) =>
            !resolutionScopeHandle.IsNil ?
                MetadataState.GetCodeElement(resolutionScopeHandle) :
                MetadataState.ExportedTypes.SingleOrDefault(exportedType => exportedType.Name.Equals(Name) && exportedType.Namespace.Equals(Namespace));
    }
}
