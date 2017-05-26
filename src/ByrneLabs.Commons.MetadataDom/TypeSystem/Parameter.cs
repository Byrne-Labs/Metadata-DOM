using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: \"{ParameterType.FullName,nq} {Name,nq}\"")]
    public class Parameter : ParameterInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<Constant> _defaultValue;
        private readonly bool _optional;

        /* 
         * Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the metadata parameter list but will have a parameter type.  If there is only one parameter 
         * type,  we don't need to worry about the position. -- Jonathan Byrne 01/11/2017
         */
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal Parameter(MemberInfo member, Type parameterType, MetadataState metadataState) : this(member, parameterType, 1, false, metadataState)
        {
        }

        /* 
         * Function pointers have parameter types but no parameter names or parameter handles -- Jonathan Byrne 01/11/2017
         */
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal Parameter(MemberInfo member, Type parameterType, int position, bool optional, MetadataState metadataState)
        {
            MetadataState = metadataState;
            Key = new CodeElementKey<Parameter>(member, position);
            _customAttributes = new Lazy<ImmutableArray<CustomAttribute>>(() => ImmutableArray<CustomAttribute>.Empty);
            Position = position;
            _optional = optional;
            _defaultValue = new Lazy<Constant>(() => null);
            Member = member;
            ParameterType = parameterType;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal Parameter(ParameterHandle metadataHandle, MethodDefinition methodDefinition, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetParameter(metadataHandle);
            Key = new CodeElementKey<Parameter>(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name) ?? string.Empty;
            Attributes = RawMetadata.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Position = RawMetadata.SequenceNumber - 1;
            Member = methodDefinition;
            ParameterType = methodDefinition.Signature.ParameterTypes[Position];
            _optional = Position > methodDefinition.Signature.RequiredParameterCount;
            _defaultValue = MetadataState.GetLazyCodeElement<Constant>(RawMetadata.GetDefaultValue());
        }

        public override ParameterAttributes Attributes { get; }

        public override object DefaultValue => _defaultValue.Value;

        public override string FullName => Name;

        public override bool HasDefaultValue => Attributes.HasFlag(ParameterAttributes.HasDefault);

        public override bool IsCompilerGenerated => ((IMemberInfo) Member).IsCompilerGenerated;

        public override bool IsSpecialName { get; }

        public override sealed MemberInfo Member { get; }

        public ParameterHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override sealed Type ParameterType { get; }

        public override sealed int Position { get; }

        public System.Reflection.Metadata.Parameter RawMetadata { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();
    }
}
