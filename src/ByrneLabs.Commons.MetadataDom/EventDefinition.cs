using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {TextSignature}")]
    public class EventDefinition : MemberBase, ICodeElementWithTypedHandle<EventDefinitionHandle, System.Reflection.Metadata.EventDefinition>
    {
        private readonly Lazy<IMethod> _addMethod;
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<IMethod> _raiseMethod;
        private readonly Lazy<IMethod> _removeMethod;
        private readonly Lazy<TypeBase> _type;

        internal EventDefinition(EventDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = (System.Reflection.Metadata.EventDefinition) MetadataState.GetTokenForHandle(metadataHandle);
            _type = new Lazy<TypeBase>(() => (TypeBase) MetadataState.GetCodeElement(RawMetadata.Type));
            Attributes = RawMetadata.Attributes;
            Name = AsString(RawMetadata.Name);
            _addMethod = MetadataState.GetLazyCodeElement<IMethod>(RawMetadata.GetAccessors().Adder);
            _raiseMethod = MetadataState.GetLazyCodeElement<IMethod>(RawMetadata.GetAccessors().Raiser);
            _removeMethod = MetadataState.GetLazyCodeElement<IMethod>(RawMetadata.GetAccessors().Remover);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Adder" />
        public IMethod AddMethod => _addMethod.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Attributes" />
        public EventAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType => AddMethod.DeclaringType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public bool IsSpecialName => Attributes.HasFlag(EventAttributes.SpecialName);

        public override MemberTypes MemberType { get; } = MemberTypes.Event;

        public override string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Raiser" />
        public IMethod RaiseMethod => _raiseMethod.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.EventAccessors.Remover" />
        public IMethod RemoveMethod => _removeMethod.Value;

        public override string TextSignature => FullName;

        /// <inheritdoc cref="System.Reflection.Metadata.EventDefinition.Type" />
        /// <summary>A <see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" /></summary>
        /*
         * HACK:  This should probably come from MetadataToken.Type but it is unclear how to create the generic context for a type specification when this is done.  It is much easier to get the type from the adder method.
         */
        public TypeBase Type => AddMethod.Parameters.Single().ParameterType;

        public System.Reflection.Metadata.EventDefinition RawMetadata { get; }

        public EventDefinitionHandle MetadataHandle { get; }
    }
}
