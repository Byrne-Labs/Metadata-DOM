﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification" />
    //[PublicAPI]
    public class MethodSpecification : MethodBase<MethodSpecification, MethodSpecificationHandle, System.Reflection.Metadata.MethodSpecification>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodBase> _method;
        private readonly Lazy<ImmutableArray<TypeBase>> _signature;

        internal MethodSpecification(MethodSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _method = new Lazy<MethodBase>(() => (MethodBase) MetadataState.GetCodeElement(new CodeElementKey(Method.GetType(), Method, Signature)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _signature = new Lazy<ImmutableArray<TypeBase>>(() => MetadataToken.DecodeSignature(MetadataState.TypeProvider, null));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string FullName { get; }

        public override IEnumerable<TypeBase> GenericArguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.Method" />
        /// <summary><see cref="MethodDefinition" /> or <see cref="MemberReferenceBase" /> specifying to which generic method this <see cref="MethodSpecification" />
        ///     refers, that is which generic method is it an instantiation of.</summary>
        public MethodBase Method => _method.Value;

        public override string Name { get; }

        public override IEnumerable<IParameter> Parameters => Method.Parameters;

        public override string TextSignature
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal ImmutableArray<TypeBase> Signature => _signature.Value;
    }
}
