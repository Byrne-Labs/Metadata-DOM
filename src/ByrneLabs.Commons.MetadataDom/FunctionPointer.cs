using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

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

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        public override TypeBase DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string FullName => null;

        public int GenericParameterCount { get; protected set; }

        public override bool IsGenericParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => null;

        public override string Namespace => null;

        public IEnumerable<TypeBase> ParameterTypes { get; protected set; }

        public int RequiredParameterCount { get; protected set; }

        public TypeBase ReturnType { get; protected set; }

        private void Initialize()
        {
            ParameterTypes = KeyValue.ParameterTypes;
            ReturnType = KeyValue.ReturnType;
            GenericParameterCount = KeyValue.GenericParameterCount;
            RequiredParameterCount = KeyValue.RequiredParameterCount;
        }
    }
}
