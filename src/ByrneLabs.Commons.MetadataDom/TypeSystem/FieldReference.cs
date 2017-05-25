using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class FieldReference : MemberReferenceBase
    {
        private readonly Lazy<TypeBase> _signature;

        internal FieldReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<FieldReference>(metadataHandle), metadataState)
        {
            _signature = new Lazy<TypeBase>(CreateFieldSignature);
        }

        public Type DeclaringType => FieldType?.DeclaringType;

        public TypeBase FieldType => _signature.Value;

        public string FullName => DeclaringType == null ? Name : $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType => MemberTypes.Field;

        public string TextSignature => $"{FullName}";

        internal TypeBase CreateFieldSignature()
        {
            TypeBase fieldSignature;
            if (Kind == MemberReferenceKind.Method)
            {
                fieldSignature = null;
            }
            else
            {
                GenericContext genericContext;
                if (Parent is MethodDefinition)
                {
                    var methodDefinitionParent = Parent as MethodDefinition;
                    genericContext = new GenericContext(this, methodDefinitionParent.DeclaringType.GenericTypeArguments, methodDefinitionParent.GetGenericArguments());
                }
                else if (Parent is ModuleReference)
                {
                    genericContext = new GenericContext(this, null, null);
                }
                else if (Parent is TypeBase)
                {
                    var typeBaseParent = Parent as TypeBase;
                    genericContext = new GenericContext(this, typeBaseParent.GenericTypeArguments, null);
                }
                else
                {
                    throw new InvalidOperationException($"The parent type {Parent?.GetType().FullName} is not recognized");
                }

                fieldSignature = RawMetadata.DecodeFieldSignature(MetadataState.TypeProvider, genericContext);
            }

            return fieldSignature;
        }
    }
}
