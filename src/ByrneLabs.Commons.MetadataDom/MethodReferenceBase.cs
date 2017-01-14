using System;
using System.Collections.Generic;
using System.Linq;
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
                    var genericContext = new GenericContext(methodDefinition.DeclaringType.GenericTypeArguments, methodDefinition.GenericTypeParameters);
                    methodSignature = RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, genericContext);
                }
                return methodSignature;
            });
        }

        public MethodDefinitionBase MethodDefinition { get; }
        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        public TypeBase DeclaringType => (TypeBase) Parent;

        public string FullName => MethodDefinition?.FullName;

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public string TextSignature => MethodDefinition?.TextSignature;

        public bool IsConstructor => this is IConstructor;

        public bool IsGenericMethod { get; } = false;

        public IEnumerable<IParameter> Parameters => MethodDefinition?.Parameters;

        public IEnumerable<GenericParameter> GenericTypeParameters => MethodDefinition?.GenericTypeParameters;
    }
}
