using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class FieldReference : MemberReferenceBase, IField
    {
        private readonly Lazy<TypeBase> _fieldSignature;

        internal FieldReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(new CodeElementKey<FieldReference>(metadataHandle), metadataState)
        {
            _fieldSignature = new Lazy<TypeBase>(CreateFieldSignature);
        }

        public FieldAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFamily
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFamilyAndAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFamilyOrAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsInitOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLiteral
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsNotSerialized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsPinvokeImpl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsPrivate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsStatic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal TypeBase FieldSignature => _fieldSignature.Value;

        public TypeBase FieldType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TypeBase DeclaringType => FieldSignature?.DeclaringType;

        public string FullName => DeclaringType == null ? Name : $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType => MemberTypes.Field;

        public string TextSignature => $"{FullName}";

        internal TypeBase CreateFieldSignature()
        {
            TypeBase fieldSignature;
            if (Kind == MemberReferenceKind.Method || Parent is TypeSpecification)
            {
                fieldSignature = null;
            }
            else
            {
                GenericContext genericContext;
                if (Parent is MethodDefinition)
                {
                    var methodDefinitionParent = Parent as MethodDefinition;
                    genericContext = new GenericContext(methodDefinitionParent.DeclaringType.GenericTypeArguments, methodDefinitionParent.GenericTypeParameters);
                }
                else if (Parent is ModuleReference)
                {
                    genericContext = new GenericContext(null, null);
                }
                else if (Parent is TypeBase)
                {
                    var typeBaseParent = Parent as TypeBase;
                    genericContext = new GenericContext(typeBaseParent.GenericTypeArguments, null);
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
