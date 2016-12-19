using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExportedType" />
    //[PublicAPI]
    public class ExportedType : TypeBase<ExportedType, ExportedTypeHandle, System.Reflection.Metadata.ExportedType>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Attributes = MetadataToken.Attributes;
            _implementation = MetadataState.GetLazyCodeElement(MetadataToken.Implementation);
            IsForwarder = MetadataToken.IsForwarder;
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(MetadataToken.NamespaceDefinition);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        public override IAssembly Assembly => MetadataState.GetCodeElement<AssemblyDefinition>(Handle.AssemblyDefinition);

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Attributes" />
        public TypeAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Implementation" />
        /// <returns>
        ///     <list type="bullet">
        ///         <item><description><see cref="AssemblyFile" /> representing another module in the assembly.</description></item>
        ///         <item>
        ///             <description><see cref="AssemblyReference" /> representing another assembly if
        ///                 <see cref="ExportedType.IsForwarder" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="ExportedType" /> representing the declaring exported type in which this was is nested.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.IsForwarder" />
        public bool IsForwarder { get; }

        public override bool IsGenericParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => AsString(MetadataToken.Name);

        public override string Namespace => AsString(MetadataToken.Namespace);

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.NamespaceDefinition" />
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;
    }
}
