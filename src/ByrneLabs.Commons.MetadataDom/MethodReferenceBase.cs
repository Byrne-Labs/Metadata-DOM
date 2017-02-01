using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class MethodReferenceBase : MemberReferenceBase, IMethodBase
    {
        private readonly Lazy<MethodSignature<TypeBase>?> _methodSignature;

        internal MethodReferenceBase(MemberReferenceHandle metadataHandle, MethodDefinitionBase methodDefinition, MetadataState metadataState) : base(new CodeElementKey<MemberReferenceBase>(metadataHandle, methodDefinition), metadataState)
        {
            MethodDefinition = methodDefinition;
            _methodSignature = new Lazy<MethodSignature<TypeBase>?>(() =>
            {
                MethodSignature<TypeBase>? methodSignature;
                if (methodDefinition == null)
                {
                    methodSignature = null;
                }
                else
                {
                    var genericContext = new GenericContext(this, methodDefinition.DeclaringType.GenericTypeArguments, methodDefinition.GenericTypeParameters);
                    methodSignature = RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, genericContext);
                }
                return methodSignature;
            });
        }

        public MethodDefinitionBase MethodDefinition { get; }

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        public TypeBase DeclaringType => Parent as TypeBase;

        public string FullName => MethodDefinition?.FullName;

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public string TextSignature => MethodDefinition?.TextSignature;

        public ImmutableArray<GenericParameter> GenericTypeParameters => MethodDefinition?.GenericTypeParameters ?? ImmutableArray<GenericParameter>.Empty;

        public bool IsConstructor => this is IConstructor;

        public bool IsGenericMethod { get; } = false;

        public ImmutableArray<IParameter> Parameters => MethodDefinition?.Parameters ?? ImmutableArray<IParameter>.Empty;
    }
}
