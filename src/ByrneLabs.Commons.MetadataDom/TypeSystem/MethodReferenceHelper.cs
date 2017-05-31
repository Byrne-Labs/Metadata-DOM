using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using MethodBaseToExpose = System.Reflection.MethodBase;
using TypeToExpose = System.Type;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal static class MethodReferenceHelper
    {
        public static ImmutableArray<TypeToExpose> GetGenericTypeParameters(MethodBaseToExpose methodDefinition)
        {
            ImmutableArray<TypeToExpose> genericTypeParameters;
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (methodDefinition is MethodInfo)
            {
                genericTypeParameters = ((MethodInfo) methodDefinition).GetGenericArguments().ToImmutableArray();
            }
            else if (methodDefinition is ConstructorInfo || methodDefinition == null)
            {
                genericTypeParameters = ImmutableArray<TypeToExpose>.Empty;
            }
            else
            {
                throw new InvalidOperationException($"MethodDefinition type {methodDefinition.GetType().FullName} was not expected");
            }

            return genericTypeParameters;
        }

        public static MethodSignature<TypeBase> GetMethodSignature(MethodBaseToExpose methodDefinition, MemberReference rawMetadata, MetadataState metadataState)
        {
            MethodSignature<TypeBase> methodSignature;
            var genericContext = new GenericContext(methodDefinition as IManagedCodeElement, methodDefinition.DeclaringType.GenericTypeArguments, methodDefinition.GetGenericArguments());
            methodSignature = rawMetadata.DecodeMethodSignature(metadataState.TypeProvider, genericContext);
            return methodSignature;
        }

        public static IEnumerable<Parameter> GetParameters(MethodBaseToExpose methodBase, MethodSignature<TypeBase> methodSignature, MetadataState metadataState)
        {
            var parameterIndex = 0;

            var parameters = methodSignature.ParameterTypes.Select(parameterType => metadataState.GetCodeElement<Parameter>(methodBase, parameterType, parameterIndex, parameterIndex++ < methodSignature.RequiredParameterCount, metadataState)).ToImmutableArray();

            return parameters;
        }
    }
}
