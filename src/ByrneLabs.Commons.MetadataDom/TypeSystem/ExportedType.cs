﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class ExportedType : EmptyTypeBase<ExportedType, ExportedTypeHandle, System.Reflection.Metadata.ExportedType>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IManagedCodeElement> _implementation;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Attributes = RawMetadata.Attributes;
            _implementation = MetadataState.GetLazyCodeElement(RawMetadata.Implementation);
            IsForwarder = RawMetadata.IsForwarder;
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.NamespaceDefinition);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Namespace = MetadataState.AssemblyReader.GetString(RawMetadata.Namespace);
            UndecoratedName = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
        }

        public override System.Reflection.Assembly Assembly => MetadataState.AssemblyDefinition;

        public TypeAttributes Attributes { get; }

        public override IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes => _customAttributes.Value;

        public override Type DeclaringType { get; }

        public object Implementation => _implementation.Value;

        public bool IsForwarder { get; }

        public override bool IsGenericParameter { get; }

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace { get; }

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        internal override string MetadataNamespace { get; }

        internal override string UndecoratedName { get; }
    }
}
