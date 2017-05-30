using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using EventInfoToExpose = System.Reflection.EventInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class MethodReference : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttributeDataToExpose>> _customAttributes;
        private readonly Lazy<ImmutableArray<TypeToExpose>> _genericTypeParameters;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<ParameterInfoToExpose> _returnParameter;

        internal MethodReference(MemberReferenceHandle metadataHandle, MetadataState metadataState)
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
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeDataToExpose>(RawMetadata.GetCustomAttributes());
            _genericTypeParameters = new Lazy<ImmutableArray<Type>>(() => MethodReferenceHelper.GetGenericTypeParameters(this));
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => MethodReferenceHelper.GetMethodSignature(this, RawMetadata, MetadataState));
            _parameters = new Lazy<IEnumerable<Parameter>>(() => MethodReferenceHelper.GetParameters(_methodSignature.Value, metadataState));
            _returnParameter = metadataState.GetLazyCodeElement<ParameterInfoToExpose>(_methodSignature.Value, ReturnType, _methodSignature.Value.ParameterTypes.Length, false, metadataState);
        }

        public override MethodAttributes Attributes => throw new NotSupportedException();

        public IEnumerable<CustomAttributeDataToExpose> CustomAttributes => _customAttributes.Value;

        public override Type DeclaringType => throw new NotSupportedException();

        public override string FullName => Name;

        public MemberReferenceHandle MetadataHandle { get; }

        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value;

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override Type ReflectedType => throw new NotSupportedException();

        public override EventInfoToExpose RelatedEvent => throw new NotSupportedException();

        public override PropertyInfoToExpose RelatedProperty => throw new NotSupportedException();

        public override ParameterInfoToExpose ReturnParameter => _returnParameter.Value;

        public TypeToExpose ReturnType => MethodSignature?.ReturnType;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotSupportedException();

        public override string TextSignature => Name;

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override MethodInfoToExpose GetBaseDefinition() => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override TypeToExpose[] GetGenericArguments() => _genericTypeParameters.Value.ToArray();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotSupportedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotSupportedException();

        public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();
    }
}
