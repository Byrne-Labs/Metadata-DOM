using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter" />
    [PublicAPI]
    public class GenericParameter : RuntimeCodeElement, ICodeElementWithHandle<GenericParameterHandle, System.Reflection.Metadata.GenericParameter>
    {
        private readonly Lazy<IEnumerable<GenericParameterConstraint>> _constraints;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _parent;

        internal GenericParameter(GenericParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetGenericParameter(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            Index = MetadataToken.Index;
            _parent = GetLazyCodeElementWithHandle(MetadataToken.Parent);
            _constraints = GetLazyCodeElementsWithHandle<GenericParameterConstraint>(MetadataToken.GetConstraints());
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Attributes" />
        public GenericParameterAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetConstraints" />
        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Index" />
        public int Index { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Parent" />
        /// <summary>
        ///     <see cref="TypeDefinition" /> or <see cref="MethodDefinition" />.</summary>
        public CodeElement Parent => _parent.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public GenericParameterHandle MetadataHandle { get; }

        public System.Reflection.Metadata.GenericParameter MetadataToken { get; }
    }
}
