using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    public class FieldReference : MemberReferenceBase, IField
    {
        internal FieldReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var parent = MetadataState.GetCodeElement(MetadataToken.Parent);
            var fieldSignature = MetadataToken.DecodeFieldSignature(MetadataState.TypeProvider, CreateGenericContext());
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

        public TypeBase FieldType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TypeBase DeclaringType => FieldSignature.DeclaringType;

        public string FullName => $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType => FieldSignature.MemberType;

        public string TextSignature => $"{FieldType.FullName} {FullName}";
    }
}
