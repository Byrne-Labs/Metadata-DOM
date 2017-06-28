using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class CustomDebugInformation : SimpleCodeElement
    {
        private static readonly Dictionary<Guid, CustomDebugInformationKinds> _customDebugInformationKindMap = new Dictionary<Guid, CustomDebugInformationKinds>
        {
            { new Guid("54FD2AC5-E925-401A-9C2A-F94F171072F8"), CustomDebugInformationKinds.AsyncMethodSteppingInformationBlob },
            { new Guid("6DA9A61E-F8C7-4874-BE62-68BC5630DF71"), CustomDebugInformationKinds.StateMachineHoistedLocalScopes },
            { new Guid("83C563C4-B4F3-47D5-B824-BA5441477EA8"), CustomDebugInformationKinds.DynamicLocalVariables },
            { new Guid("ED9FDF71-8879-4747-8ED3-FE5EDE3CE710"), CustomDebugInformationKinds.TupleElementNames },
            { new Guid("58b2eab6-209f-4e4e-a22c-b2d0f910c782"), CustomDebugInformationKinds.DefaultNamespace },
            { new Guid("755F52A8-91C5-45BE-B4B8-209571E552BD"), CustomDebugInformationKinds.EncLocalSlotMap },
            { new Guid("A643004C-0240-496F-A783-30D64F4979DE"), CustomDebugInformationKinds.EncLambdaAndClosureMap },
            { new Guid("CC110556-A091-4D38-9FEC-25AB9A351A6A"), CustomDebugInformationKinds.SourceLink },
            { new Guid("0E8A571B-6926-466E-B4AD-8AB04611F5FE"), CustomDebugInformationKinds.EmbeddedSource }
        };
        private readonly Lazy<object> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.PdbReader.GetCustomDebugInformation(metadataHandle);
            _parent = new Lazy<object>(() => MetadataState.GetCodeElement(RawMetadata.Parent));
            KindGuid = MetadataState.PdbReader.GetGuid(RawMetadata.Kind);
            Kind = _customDebugInformationKindMap[KindGuid];
            _value = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.Value, true);
        }

        public CustomDebugInformationKinds Kind { get; }

        public Guid KindGuid { get; }

        public CustomDebugInformationHandle MetadataHandle { get; }

        public object Parent => _parent.Value;

        public System.Reflection.Metadata.CustomDebugInformation RawMetadata { get; }

        public Blob Value => _value.Value;
    }
}
