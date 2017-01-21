using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Parameter" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: \"{ParameterType.FullName,nq} {Name,nq}\"")]
    public class Parameter : RuntimeCodeElement, ICodeElementWithTypedHandle<ParameterHandle, System.Reflection.Metadata.Parameter>, IParameter
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<Constant> _defaultValue;
        private readonly Lazy<Blob> _marshallingDescriptor;
        private readonly bool _optional;
        /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the metadata parameter list but will have a parameter type.  If there is only one parameter 
         * type,  we don't need to worry about the position. -- Jonathan Byrne 01/11/2017
        */

        internal Parameter(IMember member, TypeBase parameterType, MetadataState metadataState) : this(member, parameterType, 1, false, metadataState)
        {
        }

        /* Function pointers have parameter types but no parameter names or parameter handles -- Jonathan Byrne 01/11/2017
        */

        internal Parameter(IMember member, TypeBase parameterType, int position, bool optional, MetadataState metadataState) : base(new CodeElementKey<Parameter>(member, position, parameterType), metadataState)
        {
            _customAttributes = new Lazy<ImmutableArray<CustomAttribute>>(() => ImmutableArray<CustomAttribute>.Empty);
            Position = position;
            _optional = optional;
            _defaultValue = new Lazy<Constant>(() => null);
            _marshallingDescriptor = new Lazy<Blob>(() => null);
            Member = member;
            ParameterType = parameterType;
        }

        internal Parameter(ParameterHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetParameter(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Position = RawMetadata.SequenceNumber - 1;
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
            _marshallingDescriptor = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.GetMarshallingDescriptor())));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Attributes" />
        public ParameterAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetDefaultValue" />
        public Constant DefaultValue => _defaultValue.Value;

        public bool IsIndexer { get; internal set; }

        public bool IsLcid => Attributes.HasFlag(ParameterAttributes.Lcid);

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetMarshallingDescriptor" />
        public Blob MarshallingDescriptor => _marshallingDescriptor.Value;

        public System.Reflection.Metadata.Parameter RawMetadata { get; }

        public ParameterHandle MetadataHandle { get; }

        public TypeBase DeclaringType => Member.DeclaringType;

        public string FullName => Name;

        public MemberTypes MemberType => MemberTypes.Field;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.Name" />
        public string Name { get; }

        public string TextSignature => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public bool HasDefaultValue => Attributes.HasFlag(ParameterAttributes.HasDefault);

        public bool IsIn => Attributes.HasFlag(ParameterAttributes.In);

        public bool IsOptional => _optional || Attributes.HasFlag(ParameterAttributes.Optional);

        public bool IsOut => Attributes.HasFlag(ParameterAttributes.Out);

        public bool IsRetval => Attributes.HasFlag(ParameterAttributes.Retval);

        public IMember Member { get; internal set; }

        public TypeBase ParameterType { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.Parameter.SequenceNumber" />
        public int Position { get; }
    }
}
