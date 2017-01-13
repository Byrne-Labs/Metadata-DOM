using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class FunctionPointer : TypeBase<FunctionPointer, MethodSignature<TypeBase>>
    {
        private Lazy<IEnumerable<Parameter>> _parameters;

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

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        public override IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        public override TypeBase DeclaringType => null;

        public override string FullName => null;

        public int GenericParameterCount { get; protected set; }

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType => MemberTypes.Method;

        public override string Namespace => null;

        public IEnumerable<Parameter> Parameters => _parameters.Value;

        public TypeBase ReturnType { get; protected set; }

        internal override string UndecoratedName => null;

        private void Initialize()
        {
            var position = 0;
            _parameters = new Lazy<IEnumerable<Parameter>>(() => KeyValue.ParameterTypes.Select(parameterType => new Parameter(this, parameterType, ++position, position > KeyValue.RequiredParameterCount, MetadataState)).ToList());
            ReturnType = KeyValue.ReturnType;
            GenericParameterCount = KeyValue.GenericParameterCount;
        }
    }
}
