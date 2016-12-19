using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MemberReference" />
    //[PublicAPI]
    public abstract class MemberReferenceBase : RuntimeCodeElement, ICodeElementWithHandle<MemberReferenceHandle, MemberReference>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeBase> _fieldSignature;
        private readonly Lazy<MethodSignature<TypeBase>?> _methodSignature;
        private readonly Lazy<CodeElement> _parent;

        internal MemberReferenceBase(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMemberReference(metadataHandle);
            Name = AsString(MetadataToken.Name);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            Kind = MetadataToken.GetKind();
            _fieldSignature = new Lazy<TypeBase>(() => Kind == MemberReferenceKind.Method ? null : MetadataToken.DecodeFieldSignature(MetadataState.TypeProvider, CreateGenericContext()));
            _methodSignature = new Lazy<MethodSignature<TypeBase>?>(() => Kind == MemberReferenceKind.Field ? (MethodSignature<TypeBase>?) null : MetadataToken.DecodeMethodSignature(MetadataState.TypeProvider, CreateGenericContext()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.GetKind" />
        public MemberReferenceKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MemberReference.Parent" />
        /// <summary><see cref="MethodDefinition" />, <see cref="ModuleReference" />, <see cref="TypeDefinition" />, <see cref="TypeReference" />, or
        ///     <see cref="TypeSpecification" />.</summary>
        public CodeElement Parent => _parent.Value;

        internal TypeBase FieldSignature => _fieldSignature.Value;

        internal MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MemberReferenceHandle MetadataHandle { get; }

        public MemberReference MetadataToken { get; }

        internal GenericContext CreateGenericContext()
        {
            GenericContext genericContext;
            if (Parent is MethodDefinition)
            {
                var methodDefinitionParent = Parent as MethodDefinition;
                genericContext = new GenericContext(methodDefinitionParent.DeclaringType.GenericTypeArguments, methodDefinitionParent.GenericArguments);
            }
            else if (Parent is ModuleReference)
            {
                genericContext = new GenericContext(null, null);
            }
            else if (Parent is TypeBase)
            {
                var typeBaseParent = Parent as TypeBase;
                genericContext = new GenericContext(typeBaseParent.GenericTypeArguments, null);
            }
            else
            {
                throw new InvalidOperationException($"The parent type {Parent?.GetType().FullName} is not recognized");
            }

            return genericContext;
        }
    }
}
