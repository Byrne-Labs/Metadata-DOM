using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    public class FunctionPointer : TypeBase<FunctionPointer, MethodSignature<TypeBase>>
    {
        internal FunctionPointer(FunctionPointer baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal FunctionPointer(FunctionPointer genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal FunctionPointer(MethodSignature<TypeBase> signature, MetadataState metadataState) : base(signature, metadataState)
        {
            Initialize();
        }

        private Lazy<IEnumerable<Parameter>> _parameters;

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        public override TypeBase DeclaringType => null;

        public override string FullName => null;

        public int GenericParameterCount { get; protected set; }

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType => MemberTypes.Method;

        public override string Name => null;

        public override string Namespace => null;

        public IEnumerable<Parameter> Parameters => _parameters.Value;

        public TypeBase ReturnType { get; protected set; }

        private void Initialize()
        {
            var position = 0;
            _parameters = new Lazy<IEnumerable<Parameter>>(() => KeyValue.ParameterTypes.Select(parameterType => new Parameter(this, parameterType, ++position, position > KeyValue.RequiredParameterCount, MetadataState)).ToList());
            ReturnType = KeyValue.ReturnType;
            GenericParameterCount = KeyValue.GenericParameterCount;
        }
    }
}
