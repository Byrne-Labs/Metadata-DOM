﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {ImplementingType.FullName,nq} : {Interface.FullName}")]
    public class InterfaceImplementation : IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, TypeDefinition implementingType, MetadataState metadataState)
        {
            Key = new CodeElementKey<InterfaceImplementation>(metadataHandle, implementingType);
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            RawMetadata = MetadataState.AssemblyReader.GetInterfaceImplementation(metadataHandle);
            ImplementingType = implementingType;
            if (RawMetadata.Interface.Kind == HandleKind.TypeSpecification)
            {
                Interface = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Interface, ImplementingType);
            }
            else
            {
                Interface = (TypeBase) MetadataState.GetCodeElement(RawMetadata.Interface);
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public string FullName => Interface.FullName;

        public TypeDefinition ImplementingType { get; }

        public TypeBase Interface { get; }

        public InterfaceImplementationHandle MetadataHandle { get; }

        public string Name => Interface.Name;

        public System.Reflection.Metadata.InterfaceImplementation RawMetadata { get; }

        public string TextSignature => Interface.FullTextSignature;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
