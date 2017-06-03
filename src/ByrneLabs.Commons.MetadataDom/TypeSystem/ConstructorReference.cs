using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using EventInfoToExpose = System.Reflection.EventInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class ConstructorReference : ConstructorInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<IManagedCodeElement> _parent;

        internal ConstructorReference(MemberReferenceHandle metadataHandle, MetadataState metadataState)
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
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => MethodReferenceHelper.GetMethodSignature(this, RawMetadata, MetadataState));
            _parameters = new Lazy<IEnumerable<Parameter>>(() => MethodReferenceHelper.GetParameters(this, _methodSignature.Value, metadataState));
        }

        public override MethodAttributes Attributes => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public IEnumerable<CustomAttributeDataToExpose> CustomAttributes => _customAttributes.Value;

        public override TypeToExpose DeclaringType => Parent as TypeToExpose;

        public override string FullName => Name;

        public MemberReferenceHandle MetadataHandle { get; }

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override System.Reflection.Module Module => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value;

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override TypeToExpose ReflectedType => throw NotSupportedHelper.FutureVersion();

        public TypeToExpose ReturnType => MethodSignature?.ReturnType;

        public override string TextSignature => Name;

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override TypeToExpose[] GetGenericArguments() => new Type[] { };

        public override MethodImplAttributes GetMethodImplementationFlags() => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.NotValidForMetadataType(GetType());
    }
}
