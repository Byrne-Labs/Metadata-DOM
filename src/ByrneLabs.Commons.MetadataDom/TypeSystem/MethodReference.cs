using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class MethodReference : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<Type>> _genericTypeParameters;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<Parameter> _returnParameter;

        internal MethodReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : this(metadataHandle, null, metadataState)
        {
        }

        internal MethodReference(MemberReferenceHandle metadataHandle, MethodBase methodImplementationDeclaration, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodReference>(metadataHandle);
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
            _genericTypeParameters = new Lazy<ImmutableArray<Type>>(() => MethodReferenceHelper.GetGenericTypeParameters(methodImplementationDeclaration));
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => MethodReferenceHelper.GetMethodSignature(this, RawMetadata, MetadataState));
            _parameters = new Lazy<IEnumerable<Parameter>>(() => MethodReferenceHelper.GetParameters(this, _methodSignature.Value, metadataState));
            _returnParameter = metadataState.GetLazyCodeElement<Parameter>(this, ReturnType, _methodSignature.Value.ParameterTypes.Length, false, metadataState);
        }

        public override MethodAttributes Attributes => throw NotSupportedHelper.FutureVersion();

        public override Type DeclaringType => Parent as Type;

        public override string FullName => Name;

        public MemberReferenceHandle MetadataHandle { get; }

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.Module Module => throw NotSupportedHelper.FutureVersion();

        public override string Name { get; }

        public override IEnumerable<System.Reflection.ParameterInfo> Parameters => _parameters.Value;

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override EventInfo RelatedEvent => throw NotSupportedHelper.FutureVersion();

        public override PropertyInfo RelatedProperty => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.ParameterInfo ReturnParameter => _returnParameter.Value;

        public Type ReturnType => MethodSignature?.ReturnType;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw NotSupportedHelper.FutureVersion();

        public override string TextSignature => Name;

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo GetBaseDefinition() => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToList<System.Reflection.CustomAttributeData>();

        public override Type[] GetGenericArguments() => _genericTypeParameters.Value.ToArray();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw NotSupportedHelper.FutureVersion();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();
    }
}
