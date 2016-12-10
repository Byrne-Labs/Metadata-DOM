using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition" />
    [PublicAPI]
    public class EventDefinition : RuntimeCodeElement, ICodeElementWithHandle<EventDefinitionHandle, System.Reflection.Metadata.EventDefinition>
    {
        private readonly Lazy<MethodDefinition> _adder;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDefinition> _raiser;
        private readonly Lazy<MethodDefinition> _remover;
        private readonly Lazy<TypeBase> _type;

        internal EventDefinition(EventDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetEventDefinition(metadataHandle);
            _type = new Lazy<TypeBase>(() => GetCodeElementWithHandle<TypeBase>(MetadataToken.Type));
            Attributes = MetadataToken.Attributes;
            Name = AsString(MetadataToken.Name);
            _adder = GetLazyCodeElementWithHandle<MethodDefinition>(MetadataToken.GetAccessors().Adder);
            _raiser = GetLazyCodeElementWithHandle<MethodDefinition>(MetadataToken.GetAccessors().Raiser);
            _remover = GetLazyCodeElementWithHandle<MethodDefinition>(MetadataToken.GetAccessors().Remover);
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Adder" />
        public MethodDefinition Adder => _adder.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Attributes" />
        public EventAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Raiser" />
        public MethodDefinition Raiser => _raiser.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Remover" />
        public MethodDefinition Remover => _remover.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Type" />
        /// <summary>A <see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" /></summary>
        public TypeBase Type => _type.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public EventDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.EventDefinition MetadataToken { get; }
    }
}
