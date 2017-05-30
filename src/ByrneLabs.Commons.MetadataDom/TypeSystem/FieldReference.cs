using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class FieldReference : FieldInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttributeDataToExpose>> _customAttributes;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<TypeBase> _signature;

        internal FieldReference(MemberReferenceHandle metadataHandle, MetadataState metadataState)
        {
            _signature = new Lazy<TypeBase>(CreateFieldSignature);
            Key = new CodeElementKey<FieldReference>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetMemberReference(MetadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _parent = new Lazy<IManagedCodeElement>(() =>
            {
                IManagedCodeElement parent;
                if (RawMetadata.Parent.Kind == HandleKind.TypeSpecification)
                {
                    parent = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Parent, this);
                }
                else
                {
                    parent = MetadataState.GetCodeElement(RawMetadata.Parent);
                }

                return parent;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeDataToExpose>(RawMetadata.GetCustomAttributes());
        }

        public override FieldAttributes Attributes => throw new NotSupportedException();

        public override Type DeclaringType => FieldType?.DeclaringType;

        public override RuntimeFieldHandle FieldHandle => throw new NotSupportedException();

        public override TypeToExpose FieldType => _signature.Value;

        public override string FullName => DeclaringType == null ? Name : $"{DeclaringType.FullName}.{Name}";

        public MemberTypes MemberType => MemberTypes.Field;

        public MemberReferenceHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override string Name { get; }

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override TypeToExpose ReflectedType => throw new NotSupportedException();

        public override string TextSignature => $"{FullName}";

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();

        public override object GetRawConstantValue() => throw new NotSupportedException();

        public override object GetValue(object obj) => throw new NotSupportedException();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        internal TypeBase CreateFieldSignature()
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

            return RawMetadata.DecodeFieldSignature(MetadataState.TypeProvider, genericContext);
        }
    }
}
