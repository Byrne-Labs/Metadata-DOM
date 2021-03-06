﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class ManifestResource : ManifestResourceInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IManagedCodeElement> _implementation;

        internal ManifestResource(ManifestResourceHandle metadataHandle, MetadataState metadataState) : base(null, null, ResourceLocation.Embedded)
        {
            Key = new CodeElementKey<ManifestResource>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetManifestResource(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _implementation = MetadataState.GetLazyCodeElement(RawMetadata.Implementation);
            Offset = RawMetadata.Offset;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public ManifestResourceAttributes Attributes { get; }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override string FileName => throw NotSupportedHelper.FutureVersion();

        public object Implementation => _implementation.Value;

        public ManifestResourceHandle MetadataHandle { get; }

        public string Name { get; }

        public long Offset { get; }

        public System.Reflection.Metadata.ManifestResource RawMetadata { get; }

        public override System.Reflection.Assembly ReferencedAssembly => throw NotSupportedHelper.FutureVersion();

        public override ResourceLocation ResourceLocation => throw NotSupportedHelper.FutureVersion();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
