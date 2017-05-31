using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class TypeReference : EmptyTypeBase<TypeReference, TypeReferenceHandle, System.Reflection.Metadata.TypeReference>
    {
        private Lazy<object> _resolutionScope;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeReference(TypeReference baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeReference(TypeReference genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeReference(TypeReferenceHandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            Initialize();
        }

        public override System.Reflection.Assembly Assembly
        {
            get
            {
                System.Reflection.Assembly assembly;
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull -- Using try cast for all possible classes would be slower than checking the type. -- Jonathan Byrne 01/21/2017
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

        public override TypeToExpose DeclaringType => ResolutionScope is TypeReference ? (TypeBase) ResolutionScope : null;

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType => ResolutionScope is TypeReference ? MemberTypes.NestedType : MemberTypes.TypeInfo;

        public override string Namespace => IsNested ? DeclaringType?.Namespace : MetadataState.AssemblyReader.GetString(RawMetadata.Namespace);

        public object ResolutionScope => _resolutionScope.Value;

        internal override string MetadataNamespace => null;

        internal override string UndecoratedName => MetadataState.AssemblyReader.GetString(RawMetadata.Name);

        private void Initialize()
        {
            _resolutionScope = new Lazy<object>(() => !RawMetadata.ResolutionScope.IsNil ? MetadataState.GetCodeElement(RawMetadata.ResolutionScope) : (object) MetadataState.DefinedTypes.SingleOrDefault(exportedType => exportedType.Name.Equals(Name) && exportedType.Namespace.Equals(Namespace)));
        }
    }
}
