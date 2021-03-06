﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Constructor.DeclaringType.FullName}")]
    public class CustomAttribute : CustomAttributeData, IManagedCodeElement
    {
        private readonly Lazy<ConstructorInfo> _constructor;
        private readonly Lazy<ImmutableList<CustomAttributeTypedArgument>> _constructorArguments;
        private readonly Lazy<ImmutableList<CustomAttributeNamedArgument>> _namedArguments;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<CustomAttributeValue<TypeBase>?> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            Key = new CodeElementKey<CustomAttribute>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetCustomAttribute(metadataHandle);
            _constructor = MetadataState.GetLazyCodeElement<ConstructorInfo>(RawMetadata.Constructor, null);
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            _value = new Lazy<CustomAttributeValue<TypeBase>?>(() => RawMetadata.DecodeValue(MetadataState.TypeProvider));
            _constructorArguments = new Lazy<ImmutableList<CustomAttributeTypedArgument>>(() => Value.HasValue ? Value.Value.FixedArguments.Select(argument => new CustomAttributeTypedArgument(argument.Type, argument.Value)).ToImmutableList() : ImmutableList<CustomAttributeTypedArgument>.Empty);
            _namedArguments = new Lazy<ImmutableList<CustomAttributeNamedArgument>>(() =>
            {
                var namedArguements = new List<CustomAttributeNamedArgument>();
                if (Value.HasValue)
                {
                    foreach (var argument in Value.Value.NamedArguments)
                    {
                        MemberInfo memberInfo;
                        if (argument.Kind == CustomAttributeNamedArgumentKind.Field)
                        {
                            memberInfo = AttributeType.GetFields().Single(field => field.Name.Equals(argument.Name));
                        }
                        else
                        {
                            memberInfo = AttributeType.GetProperties().Single(property => property.Name.Equals(argument.Name));
                        }
                        namedArguements.Add(new CustomAttributeNamedArgument(memberInfo, argument.Value));
                    }
                }

                return namedArguements.ToImmutableList();
            });
        }

        public Type AttributeType => Constructor.DeclaringType;

        public override System.Reflection.ConstructorInfo Constructor => _constructor.Value;

        public override IList<CustomAttributeTypedArgument> ConstructorArguments => _constructorArguments.Value;

        public CustomAttributeHandle MetadataHandle { get; }

        public override IList<CustomAttributeNamedArgument> NamedArguments => _namedArguments.Value;

        public System.Reflection.Metadata.CustomAttribute RawMetadata { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        internal IManagedCodeElement Parent => _parent.Value;

        internal CustomAttributeValue<TypeBase>? Value => _value.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
