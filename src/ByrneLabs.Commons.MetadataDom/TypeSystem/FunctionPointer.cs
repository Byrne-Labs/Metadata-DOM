using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using AssemblyToExpose = System.Reflection.Assembly;

#else
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class FunctionPointer : EmptyTypeBase
    {
        private Lazy<ImmutableArray<Parameter>> _parameters;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal FunctionPointer(FunctionPointer baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<FunctionPointer>(baseType, typeElementModifier))
        {
            MethodSignature = baseType.MethodSignature;
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal FunctionPointer(FunctionPointer genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<FunctionPointer>(genericTypeDefinition, genericTypeArguments))
        {
            MethodSignature = genericTypeDefinition.MethodSignature;
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal FunctionPointer(MethodSignature<TypeBase> methodSignature, MetadataState metadataState) : base(new CodeElementKey<FunctionPointer>(methodSignature), metadataState)
        {
            MethodSignature = methodSignature;
            Initialize();
        }

        public override AssemblyToExpose Assembly => MetadataState.AssemblyDefinition;

        public int GenericParameterCount { get; protected set; }

        public override MemberTypes MemberType => MemberTypes.Method;

        public MethodSignature<TypeBase> MethodSignature { get; }

        public ImmutableArray<Parameter> Parameters => _parameters.Value;

        public TypeBase ReturnType { get; protected set; }

        private void Initialize()
        {
            var position = 0;
            _parameters = new Lazy<ImmutableArray<Parameter>>(() => MethodSignature.ParameterTypes.Select(parameterType => new Parameter(this, parameterType, ++position, position > MethodSignature.RequiredParameterCount, MetadataState)).ToImmutableArray());
            ReturnType = MethodSignature.ReturnType;
            GenericParameterCount = MethodSignature.GenericParameterCount;
        }
    }
}
