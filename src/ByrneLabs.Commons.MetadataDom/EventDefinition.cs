using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition" />
    public class EventDefinition : CodeElementWithHandle
    {
        private readonly Lazy<MethodDefinition> _adder;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<string> _name;
        private readonly Lazy<MethodDefinition> _raiser;
        private readonly Lazy<MethodDefinition> _remover;
        private readonly Lazy<CodeElement> _type;

        internal EventDefinition(EventDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var eventDefinition = Reader.GetEventDefinition(metadataHandle);
            _type = new Lazy<CodeElement>(() => GetCodeElement(eventDefinition.Type));
            Attributes = eventDefinition.Attributes;
            _name = new Lazy<string>(() => AsString(eventDefinition.Name));
            _adder = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(eventDefinition.GetAccessors().Adder));
            _raiser = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(eventDefinition.GetAccessors().Raiser));
            _remover = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(eventDefinition.GetAccessors().Remover));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(eventDefinition.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Adder" />
        public MethodDefinition Adder => _adder.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Attributes" />
        public EventAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Raiser" />
        public MethodDefinition Raiser => _raiser.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Remover" />
        public MethodDefinition Remover => _remover.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Type" />
        public CodeElement Type => _type.Value;
    }
}
