using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using MethodBaseToExpose = System.Reflection.MethodBase;
using TypeToExpose = System.Type;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public abstract class MethodReferenceBase : MemberReferenceBase
    {
        private readonly Lazy<ImmutableArray<TypeToExpose>> _genericTypeParameters;
        private readonly Lazy<MethodSignature<TypeBase>?> _methodSignature;
        private readonly Lazy<ImmutableArray<ParameterInfo>> _parameters;

        internal MethodReferenceBase(MemberReferenceHandle metadataHandle, MethodBaseToExpose methodDefinition, MetadataState metadataState) : base(new CodeElementKey<MemberReferenceBase>(metadataHandle, methodDefinition), metadataState)
        {
            MethodDefinition = methodDefinition;
            _genericTypeParameters = new Lazy<ImmutableArray<Type>>(() =>
            {
                ImmutableArray<Type> genericTypeParameters;
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (methodDefinition is MethodInfo)
                {
                    genericTypeParameters = ((MethodInfo) methodDefinition).GetGenericArguments().ToImmutableArray();
                }
                else if (methodDefinition is ConstructorInfo)
                {
                    genericTypeParameters = ImmutableArray<Type>.Empty;
                }
                else
                {
                    throw new InvalidOperationException($"MethodDefinition type {methodDefinition.GetType().FullName} was not expected");
                }

                return genericTypeParameters;
            });
            _methodSignature = new Lazy<MethodSignature<TypeBase>?>(() =>
            {
                MethodSignature<TypeBase>? methodSignature;
                if (methodDefinition == null)
                {
                    methodSignature = null;
                }
                else
                {
                    var genericContext = new GenericContext(this, methodDefinition.DeclaringType.GenericTypeArguments, _genericTypeParameters.Value);
                    methodSignature = RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, genericContext);
                }
                return methodSignature;
            });
            _parameters = new Lazy<ImmutableArray<ParameterInfo>>(() =>
            {
                ImmutableArray<ParameterInfo> parameters;
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (methodDefinition is MethodInfo)
                {
                    parameters = ((MethodInfo) methodDefinition).GetParameters().Cast<ParameterInfo>().ToImmutableArray();
                }
                else if (methodDefinition is ConstructorInfo)
                {
                    parameters = ((ConstructorInfo) methodDefinition).GetParameters().Cast<ParameterInfo>().ToImmutableArray();
                }
                else
                {
                    throw new InvalidOperationException($"MethodDefinition type {methodDefinition.GetType().FullName} was not expected");
                }

                return parameters;
            });
        }

        public abstract bool IsConstructor { get; }

        public TypeBase DeclaringType => Parent as TypeBase;

        public bool IsGenericMethod => false;

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public MethodBase MethodDefinition { get; }

        public ImmutableArray<ParameterInfoToExpose> Parameters => _parameters.Value.CastArray<ParameterInfoToExpose>();

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;
    }
}
