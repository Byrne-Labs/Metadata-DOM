using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeReference" />
    //[PublicAPI]
    public class TypeReference : TypeBase<TypeReference, TypeReferenceHandle, System.Reflection.Metadata.TypeReference>
    {
        private Lazy<object> _resolutionScope;

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

        public override IAssembly Assembly
        {
            get
            {
                IAssembly assembly;
                if (ResolutionScope is AssemblyReference)
                {
                    assembly = (AssemblyReference) ResolutionScope;
                }
                else if (ResolutionScope is TypeReference)
                {
                    assembly = ((TypeReference) ResolutionScope).Assembly;
                }
                else if (ResolutionScope is ModuleReference)
                {
                    assembly = ((ModuleReference) ResolutionScope).Assembly;
                }
                else if (ResolutionScope is ModuleDefinition)
                {
                    assembly = ((ModuleDefinition) ResolutionScope).Assembly;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid resolution scope {ResolutionScope?.GetType()}");
                }

                return assembly;
            }
        }

        public override TypeBase DeclaringType => ResolutionScope is TypeReference ? (TypeBase) ResolutionScope : null;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType => ResolutionScope is TypeReference ? MemberTypes.NestedType : MemberTypes.TypeInfo;

        public override string Name => AsString(MetadataToken.Name);

        public override string Namespace => AsString(MetadataToken.Namespace);

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
        ///                 <see cref="Metadata.ExportedTypes" /> for a matching <see cref="TypeBase.Namespace" /> and <see cref="TypeBase.Name" />.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public object ResolutionScope => _resolutionScope.Value;

        private void Initialize()
        {
            _resolutionScope = new Lazy<object>(() => LoadResolutionScope(MetadataToken.ResolutionScope));
        }

        private object LoadResolutionScope(EntityHandle resolutionScopeHandle) =>
            !resolutionScopeHandle.IsNil ?
                MetadataState.GetCodeElement(resolutionScopeHandle) :
                MetadataState.DefinedTypes.SingleOrDefault(exportedType => exportedType.Name.Equals(Name) && exportedType.Namespace.Equals(Namespace));
    }
}
