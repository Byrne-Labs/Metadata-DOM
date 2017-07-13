using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class FieldReference : FieldInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
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
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override FieldAttributes Attributes => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override Type DeclaringType => FieldType?.DeclaringType;

        public override RuntimeFieldHandle FieldHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override Type FieldType => _signature.Value;

        public override string FullName => DeclaringType == null ? Name : $"{DeclaringType.FullName}.{Name}";

        public override string FullTextSignature => $"{FullName}";

        public MemberTypes MemberType => MemberTypes.Field;

        public MemberReferenceHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override string Name { get; }

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.NotValidForMetadata();

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => throw NotSupportedHelper.FutureVersion();

        public override string SourceCode => throw NotSupportedHelper.FutureVersion();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override object GetRawConstantValue() => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override object GetValue(object obj) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

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
