using System;
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
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullTextSignature}")]
    public class EventDefinition : EventInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttributeData>> _customAttributes;
        private readonly Lazy<IEnumerable<MetadataDom.SequencePoint>> _sequencePoints;

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
            _sequencePoints = new Lazy<IEnumerable<MetadataDom.SequencePoint>>(() =>
            {
                var sequencePoints = new List<MetadataDom.SequencePoint>();
                if ((AddMethod as MethodDefinition)?.SequencePoints != null)
                {
                    sequencePoints.AddRange((AddMethod as MethodDefinition)?.SequencePoints);
                }
                if ((RemoveMethod as MethodDefinition)?.SequencePoints != null)
                {
                    sequencePoints.AddRange((RemoveMethod as MethodDefinition)?.SequencePoints);
                }
                if ((RaiseMethod as MethodDefinition)?.SequencePoints != null)
                {
                    sequencePoints.AddRange((RaiseMethod as MethodDefinition)?.SequencePoints);
                }

                return sequencePoints.OrderBy(sp => sp.Document.Name).ThenBy(sp => sp.StartLine).ThenBy(sp => sp.StartColumn).ToImmutableArray();
            });
        }

        public override System.Reflection.MethodInfo AddMethod { get; }

        public override EventAttributes Attributes => RawMetadata.Attributes;

        public override Type DeclaringType => AddMethod.DeclaringType;

        /*
         * HACK:  This should probably come from MetadataToken.Type but it is unclear how to create the generic context for a type specification when this is done.  It is much easier to get the type from the adder method.
         */
        public override Type EventHandlerType => AddMethod.GetParameters().Single().ParameterType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override string FullTextSignature => FullName;

        public override bool IsMulticast
        {
            get
            {
                var multicast = false;
                var baseType = EventHandlerType;
                while (baseType != null && !multicast)
                {
                    if (!(baseType is TypeDefinition))
                    {
                        // ReSharper disable once PossibleMistakenCallToGetType.2
                        throw NotSupportedHelper.NotValidForMetadataType(baseType.GetType());
                    }

                    multicast = baseType.FullName.Equals("System.MulticastDelegate");
                    baseType = baseType.BaseType;
                }

                return multicast;
            }
        }

        public override MemberTypes MemberType { get; } = MemberTypes.Event;

        public EventDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override System.Reflection.MethodInfo RaiseMethod { get; }

        public System.Reflection.Metadata.EventDefinition RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.MethodInfo RemoveMethod { get; }

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => _sequencePoints.Value;

        public override string SourceCode => throw NotSupportedHelper.FutureVersion();

        public override string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo GetAddMethod(bool nonPublic) => !nonPublic && !AddMethod.IsPublic ? null : AddMethod;

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override System.Reflection.MethodInfo GetRaiseMethod(bool nonPublic) => !nonPublic && !RaiseMethod.IsPublic ? null : RaiseMethod;

        public override System.Reflection.MethodInfo GetRemoveMethod(bool nonPublic) => !nonPublic && !RemoveMethod.IsPublic ? null : RemoveMethod;

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();
    }
}
