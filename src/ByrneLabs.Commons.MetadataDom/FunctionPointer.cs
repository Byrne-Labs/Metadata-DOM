﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class FunctionPointer : TypeBase<FunctionPointer, MethodSignature<TypeBase>>
    {
        private Lazy<ImmutableArray<Parameter>> _parameters;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal FunctionPointer(FunctionPointer baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal FunctionPointer(FunctionPointer genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal FunctionPointer(MethodSignature<TypeBase> signature, MetadataState metadataState) : base(signature, metadataState)
        {
            Initialize();
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType => null;

        public override string FullName => null;

        public int GenericParameterCount { get; protected set; }

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType => MemberTypes.Method;

        public override string Namespace => null;

        public ImmutableArray<Parameter> Parameters => _parameters.Value;

        public TypeBase ReturnType { get; protected set; }

        protected override string MetadataNamespace { get; } = null;

        internal override string UndecoratedName => null;

        private void Initialize()
        {
            var position = 0;
            _parameters = new Lazy<ImmutableArray<Parameter>>(() => KeyValue.ParameterTypes.Select(parameterType => new Parameter(this, parameterType, ++position, position > KeyValue.RequiredParameterCount, MetadataState)).ToImmutableArray());
            ReturnType = KeyValue.ReturnType;
            GenericParameterCount = KeyValue.GenericParameterCount;
        }
    }
}
