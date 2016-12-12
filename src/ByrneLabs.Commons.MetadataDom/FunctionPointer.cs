using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

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


        private void Initialize()
        {
            ParameterTypes = KeyValue.ParameterTypes;
            ReturnType = KeyValue.ReturnType;
            GenericParameterCount = KeyValue.GenericParameterCount;
            RequiredParameterCount = KeyValue.RequiredParameterCount;
        }

        public int GenericParameterCount { get; protected set; }

        public IEnumerable<TypeBase> ParameterTypes { get; protected set; }

        public int RequiredParameterCount { get; protected set; }

        public TypeBase ReturnType { get; protected set; }
    }
}
