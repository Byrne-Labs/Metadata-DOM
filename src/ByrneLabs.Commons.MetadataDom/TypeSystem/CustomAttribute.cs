using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Reflection;
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using TypeToExpose = System.Type;
using CustomAttributeTypedArgumentToExpose = System.Reflection.CustomAttributeTypedArgument;
using CustomAttributeNamedArgumentToExpose = System.Reflection.CustomAttributeNamedArgument;

#else
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using CustomAttributeTypedArgumentToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeTypedArgument;
using CustomAttributeNamedArgumentToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeNamedArgument;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Constructor.DeclaringType.FullName}")]
    public class CustomAttribute : CustomAttributeData, IManagedCodeElement
    {
        private readonly Lazy<ConstructorInfoToExpose> _constructor;
        private readonly Lazy<ImmutableList<CustomAttributeTypedArgumentToExpose>> _constructorArguments;
        private readonly Lazy<ImmutableList<CustomAttributeNamedArgumentToExpose>> _namedArguments;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<CustomAttributeValue<TypeBase>?> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            Key = new CodeElementKey<CustomAttribute>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetCustomAttribute(metadataHandle);
            _constructor = MetadataState.GetLazyCodeElement<ConstructorInfoToExpose>(RawMetadata.Constructor, null);
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            _value = new Lazy<CustomAttributeValue<TypeBase>?>(() =>
            {
                CustomAttributeValue<TypeBase>? value;

                /*
                 * I cannot figure out why, but the DecodeValue call will throw an exception if the argument kind is not field or property. -- Jonathan Byrne 12/19/2016
                 */
                var valueKind = (CustomAttributeNamedArgumentKind) MetadataState.AssemblyReader.GetBlobReader(RawMetadata.Value).ReadSerializationTypeCode();
                if (valueKind == CustomAttributeNamedArgumentKind.Field || valueKind == CustomAttributeNamedArgumentKind.Property)
                {
                    value = RawMetadata.DecodeValue(MetadataState.TypeProvider);
                }
                else
                {
                    value = null;
                }

                return value;
            });
            _constructorArguments = new Lazy<ImmutableList<CustomAttributeTypedArgument>>(() => Value.HasValue ? Value.Value.FixedArguments.Select(argument => new CustomAttributeTypedArgument(argument.Type, argument.Value)).ToImmutableList() : ImmutableList<CustomAttributeTypedArgument>.Empty);
            _namedArguments = new Lazy<ImmutableList<CustomAttributeNamedArgument>>(() => Value.HasValue ? Value.Value.NamedArguments.Select(argument => new CustomAttributeNamedArgument(argument.Type, argument.Value)).ToImmutableList() : ImmutableList<CustomAttributeNamedArgument>.Empty);
        }

        public TypeToExpose AttributeType => Constructor.DeclaringType;

        public override ConstructorInfoToExpose Constructor => _constructor.Value;

        public override IList<CustomAttributeTypedArgumentToExpose> ConstructorArguments => _constructorArguments.Value;

        public string FullName => throw new NotSupportedException();

        public CustomAttributeHandle MetadataHandle { get; }

        public string Name => throw new NotSupportedException();

        public override IList<CustomAttributeNamedArgumentToExpose> NamedArguments => _namedArguments.Value;

        public System.Reflection.Metadata.CustomAttribute RawMetadata { get; }

        public string TextSignature => throw new NotSupportedException();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        internal IManagedCodeElement Parent => _parent.Value;

        internal CustomAttributeValue<TypeBase>? Value => _value.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
