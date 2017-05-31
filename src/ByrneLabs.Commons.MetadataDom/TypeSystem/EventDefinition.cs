using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {TextSignature}")]
    public partial class EventDefinition : EventInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttributeData>> _customAttributes;

        internal EventDefinition(EventDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            Key = new CodeElementKey<EventDefinition>(metadataHandle);
            MetadataHandle = metadataHandle;
            RawMetadata = (System.Reflection.Metadata.EventDefinition) MetadataState.GetRawMetadataForHandle(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            if (!RawMetadata.GetAccessors().Adder.IsNil)
            {
                AddMethod = MetadataState.GetCodeElement<MethodInfo>(RawMetadata.GetAccessors().Adder);
            }
            if (!RawMetadata.GetAccessors().Raiser.IsNil)
            {
                RaiseMethod = MetadataState.GetCodeElement<MethodInfo>(RawMetadata.GetAccessors().Raiser);
            }
            if (!RawMetadata.GetAccessors().Remover.IsNil)
            {
                RemoveMethod = MetadataState.GetCodeElement<MethodInfo>(RawMetadata.GetAccessors().Remover);
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
        }

        public override MethodInfoToExpose AddMethod { get; }

        public override EventAttributes Attributes => RawMetadata.Attributes;

        public override TypeToExpose DeclaringType => AddMethod.DeclaringType;

        /*
         * HACK:  This should probably come from MetadataToken.Type but it is unclear how to create the generic context for a type specification when this is done.  It is much easier to get the type from the adder method.
         */
        public override TypeToExpose EventHandlerType => AddMethod.GetParameters().Single().ParameterType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override bool IsMulticast
        {
            get
            {
                var multicast = false;
                var baseType = EventHandlerType;
                while (baseType != null && !multicast)
                {
                    multicast = baseType.FullName.Equals("System.MulticastDelegate");
                    baseType = baseType.BaseType;
                }

                return multicast;
            }
        }

        public override bool IsSpecialName => Attributes.HasFlag(EventAttributes.SpecialName);

        public override MemberTypes MemberType { get; } = MemberTypes.Event;

        public EventDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override MethodInfoToExpose RaiseMethod { get; }

        public System.Reflection.Metadata.EventDefinition RawMetadata { get; }

        public override TypeToExpose ReflectedType => throw new NotSupportedException();

        public override MethodInfoToExpose RemoveMethod { get; }

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override MethodInfoToExpose GetAddMethod(bool nonPublic) => !nonPublic && !AddMethod.IsPublic ? null : AddMethod;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override MethodInfoToExpose GetRaiseMethod(bool nonPublic) => !nonPublic && !RaiseMethod.IsPublic ? null : RaiseMethod;

        public override MethodInfoToExpose GetRemoveMethod(bool nonPublic) => !nonPublic && !RemoveMethod.IsPublic ? null : RemoveMethod;
    }
#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class EventDefinition
    {
        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();
    }

#else
    public partial class EventDefinition
    {
    }
#endif
}
