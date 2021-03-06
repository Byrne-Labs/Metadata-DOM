﻿using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class LocalConstant : LocalConstantInfo, IManagedCodeElement
    {
        private readonly Lazy<Blob> _signature;

        internal LocalConstant(LocalConstantHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<LocalConstant>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.PdbReader.GetLocalConstant(MetadataHandle);
            Name = MetadataState.PdbReader.GetString(RawMetadata.Name);
            _signature = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.Signature, true);
        }

        public override System.Reflection.TypeInfo ConstantType { get; }

        public LocalConstantHandle MetadataHandle { get; }

        public override string Name { get; }

        public System.Reflection.Metadata.LocalConstant RawMetadata { get; }

        public Blob Signature => _signature.Value;

        public override object Value { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
