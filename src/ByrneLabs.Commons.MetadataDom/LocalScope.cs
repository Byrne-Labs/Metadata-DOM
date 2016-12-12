using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalScope" />
    [PublicAPI]
    public class LocalScope : DebugCodeElement, ICodeElementWithHandle<LocalScopeHandle, System.Reflection.Metadata.LocalScope>
    {
        private readonly Lazy<IEnumerable<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<IEnumerable<LocalConstant>> _localConstants;
        private readonly Lazy<IEnumerable<LocalVariable>> _localVariables;
        private readonly Lazy<MethodDefinitionBase> _method;

        internal LocalScope(LocalScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetLocalScope(metadataHandle);
            EndOffset = MetadataToken.EndOffset;
            _importScope = MetadataState.GetLazyCodeElement<ImportScope>(MetadataToken.ImportScope);
            Length = MetadataToken.Length;
            _method = MetadataState.GetLazyCodeElement<MethodDefinitionBase>(MetadataToken.Method);
            StartOffset = MetadataToken.StartOffset;
            _children = new Lazy<IEnumerable<LocalScope>>(LoadChildren);
            _localConstants = MetadataState.GetLazyCodeElements<LocalConstant>(MetadataToken.GetLocalConstants());
            _localVariables = MetadataState.GetLazyCodeElements<LocalVariable>(MetadataToken.GetLocalVariables());
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

        public Handle DowncastMetadataHandle => MetadataHandle;

        public LocalScopeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.LocalScope MetadataToken { get; }

        private IEnumerable<LocalScope> LoadChildren()
        {
            var childrenHandles = new List<LocalScopeHandle>();
            var childrenEnumerator = MetadataToken.GetChildren();
            while (childrenEnumerator.MoveNext())
            {
                childrenHandles.Add(childrenEnumerator.Current);
            }

            return MetadataState.GetCodeElements<LocalScope>(childrenHandles);
        }
    }
}
