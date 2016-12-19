using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition" />
    //[PublicAPI]
    public class EventDefinition : MemberBase<EventDefinition, EventDefinitionHandle, System.Reflection.Metadata.EventDefinition>
    {
        private readonly Lazy<IMethod> _addMethod;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IMethod> _raiseMethod;
        private readonly Lazy<IMethod> _removeMethod;
        private readonly Lazy<TypeBase> _type;

        internal EventDefinition(EventDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _type = new Lazy<TypeBase>(() => (TypeBase)MetadataState.GetCodeElement(MetadataToken.Type));
            Attributes = MetadataToken.Attributes;
            Name = AsString(MetadataToken.Name);
            _addMethod = MetadataState.GetLazyCodeElement<IMethod>(MetadataToken.GetAccessors().Adder);
            _raiseMethod = MetadataState.GetLazyCodeElement<IMethod>(MetadataToken.GetAccessors().Raiser);
            _removeMethod = MetadataState.GetLazyCodeElement<IMethod>(MetadataToken.GetAccessors().Remover);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Adder" />
        public IMethod AddMethod => _addMethod.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Attributes" />
        public EventAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType => AddMethod.DeclaringType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public bool IsSpecialName => Attributes.HasFlag(EventAttributes.SpecialName);

        public override MemberTypes MemberType { get; } = MemberTypes.Event;

        public override string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Raiser" />
        public IMethod RaiseMethod => _raiseMethod.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Remover" />
        public IMethod RemoveMethod => _removeMethod.Value;

        public override string TextSignature => $"{Type.FullName} {FullName}";

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Type" />
        /// <summary>A <see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" /></summary>
        public TypeBase Type => _type.Value;
    }
}
