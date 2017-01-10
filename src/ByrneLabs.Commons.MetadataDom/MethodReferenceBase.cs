using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class MethodReferenceBase : MemberReferenceBase, IMethodBase
    {
        private readonly MethodDefinitionBase _methodDefinition;
        private readonly Lazy<MethodSignature<TypeBase>?> _methodSignature;

        internal MethodReferenceBase(MemberReferenceHandle metadataHandle, MethodDefinitionBase methodDefinition, MetadataState metadataState) : base(new CodeElementKey<MemberReferenceBase>(metadataHandle, methodDefinition), metadataState)
        {
            _methodDefinition = methodDefinition;
            _methodSignature = new Lazy<MethodSignature<TypeBase>?>(() =>
            {
                MethodSignature<TypeBase>? methodSignature;
                if (methodDefinition == null)
                {
                    methodSignature = null;
                }
                else
                {
                    var genericContext = new GenericContext(methodDefinition.DeclaringType.GenericTypeArguments, methodDefinition.GenericArguments);
                    methodSignature = MetadataToken.DecodeMethodSignature(MetadataState.TypeProvider, genericContext);
                }
                return methodSignature;
            });
        }

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        public TypeBase DeclaringType => (TypeBase) Parent;

        public string FullName => _methodDefinition?.FullName;

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public string TextSignature => _methodDefinition?.TextSignature;

        public bool ContainsGenericParameters { get; } = false;

        public IEnumerable<TypeBase> GenericArguments { get; } = new List<TypeBase>();

        public bool IsConstructor => this is IConstructor;

        public bool IsGenericMethod { get; } = false;

        public IEnumerable<IParameter> Parameters => _methodDefinition?.Parameters;
    }
}
