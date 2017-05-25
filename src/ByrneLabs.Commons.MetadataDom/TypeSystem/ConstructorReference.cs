using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using ModuleToExpose = System.Reflection.Module;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;
using MethodBodyToExpose = System.Reflection.MethodBody;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;
using MethodBodyToExpose = ByrneLabs.Commons.MetadataDom.MethodBody;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public partial class ConstructorReference : ConstructorInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<Type>> _genericTypeParameters;
        private readonly Lazy<MethodSignature<TypeBase>?> _methodSignature;
        private readonly Lazy<ImmutableArray<ParameterInfoToExpose>> _parameters;
        private readonly Lazy<IManagedCodeElement> _parent;

        internal ConstructorReference(MemberReferenceHandle metadataHandle, MetadataState metadataState) : this(metadataHandle, null, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "The second parameter can only be a ConstructorDefinition")]
        internal ConstructorReference(MemberReferenceHandle metadataHandle, ConstructorDefinition constructorDefinition, MetadataState metadataState)
        {
            Key = new CodeElementKey<ConstructorReference>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = (MemberReferenceHandle) Key.UpcastHandle;
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
            Kind = RawMetadata.GetKind();

            ConstructorDefinition = constructorDefinition;
            _genericTypeParameters = new Lazy<ImmutableArray<Type>>(() =>
            {
                ImmutableArray<Type> genericTypeParameters;
                genericTypeParameters = ImmutableArray<Type>.Empty;

                return genericTypeParameters;
            });
            _methodSignature = new Lazy<MethodSignature<TypeBase>?>(() =>
            {
                MethodSignature<TypeBase>? methodSignature;
                if (constructorDefinition == null)
                {
                    methodSignature = null;
                }
                else
                {
                    var genericContext = new GenericContext(this, constructorDefinition.DeclaringType.GenericTypeArguments, _genericTypeParameters.Value);
                    methodSignature = RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, genericContext);
                }
                return methodSignature;
            });
            _parameters = new Lazy<ImmutableArray<ParameterInfoToExpose>>(() =>
            {
                ImmutableArray<ParameterInfoToExpose> parameters;
                // ReSharper disable once RedundantEnumerableCastCall
                parameters = constructorDefinition.GetParameters().Cast<ParameterInfoToExpose>().ToImmutableArray();

                return parameters;
            });
        }

        public override MethodAttributes Attributes => throw new NotSupportedException();

        public MethodBase ConstructorDefinition { get; }

        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeToExpose DeclaringType => Parent as TypeBase;

        public override string FullName => Name;

        public bool IsGenericMethod => false;

        public MemberReferenceKind Kind { get; }

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public MemberReferenceHandle MetadataHandle { get; }

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value.ToImmutableList();

        public object Parent => _parent.Value;

        public MemberReference RawMetadata { get; }

        public override Type ReflectedType => null;

        public override string TextSignature => Name;

        protected MethodSignature<TypeBase>? MethodSignature => _methodSignature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override TypeToExpose[] GetGenericArguments() => new Type[] { };

        public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotSupportedException();

        public override ParameterInfoToExpose[] GetParameters() => _parameters.Value.ToArray();
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class ConstructorReference
    {
        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotSupportedException();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();
    }

#else
    public partial class ConstructorReference
    {
    }

#endif
}
