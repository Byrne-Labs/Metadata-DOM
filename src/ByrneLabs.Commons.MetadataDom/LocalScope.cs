using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalScope" />
    public class LocalScope : DebugCodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<IEnumerable<LocalConstant>> _localConstants;
        private readonly Lazy<IEnumerable<LocalVariable>> _localVariables;
        private readonly Lazy<MethodDefinitionBase> _method;

        internal LocalScope(LocalScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var localScope = Reader.GetLocalScope(metadataHandle);
            EndOffset = localScope.EndOffset;
            _importScope = new Lazy<ImportScope>(() => GetCodeElement<ImportScope>(localScope.ImportScope));
            Length = localScope.Length;
            _method = new Lazy<MethodDefinitionBase>(() => GetCodeElement<MethodDefinitionBase>(localScope.Method));
            StartOffset = localScope.StartOffset;
            _children = new Lazy<IEnumerable<LocalScope>>(LoadChildren);
            _localConstants = new Lazy<IEnumerable<LocalConstant>>(() => GetCodeElements<LocalConstant>(localScope.GetLocalConstants()));
            _localVariables = new Lazy<IEnumerable<LocalVariable>>(() => GetCodeElements<LocalVariable>(localScope.GetLocalVariables()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.GetChildren" />
        public IEnumerable<LocalScope> Children => _children.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.EndOffset" />
        public int EndOffset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.ImportScope" />
        public ImportScope ImportScope => _importScope.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.Length" />
        public int Length { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.GetLocalConstants" />
        public IEnumerable<LocalConstant> LocalConstants => _localConstants.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.GetLocalVariables" />
        public IEnumerable<LocalVariable> LocalVariables => _localVariables.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.Method" />
        public MethodDefinitionBase Method => _method.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalScope.StartOffset" />
        public int StartOffset { get; }

        private IEnumerable<LocalScope> LoadChildren()
        {
            var localScope = Reader.GetLocalScope((LocalScopeHandle) ((ICodeElementWithHandle) this).MetadataHandle);

            var childrenHandles = new List<LocalScopeHandle>();
            var childrenEnumerator = localScope.GetChildren();
            while (childrenEnumerator.MoveNext())
            {
                childrenHandles.Add(childrenEnumerator.Current);
            }

            return GetCodeElements<LocalScope>(childrenHandles);
        }
    }
}
