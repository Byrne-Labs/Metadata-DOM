using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class EventDefinition : CodeElementWithHandle
    {
        private readonly Lazy<MethodDefinition> _adder;
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
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
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(eventDefinition.GetCustomAttributes()));
        }

        public MethodDefinition Adder => _adder.Value;

        public EventAttributes Attributes { get; }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public string Name => _name.Value;

        public MethodDefinition Raiser => _raiser.Value;

        public MethodDefinition Remover => _remover.Value;

        public CodeElement Type => _type.Value;
    }
}
