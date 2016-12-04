using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class LocalScope : DebugCodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<IReadOnlyList<LocalConstant>> _localConstants;
        private readonly Lazy<IReadOnlyList<LocalVariable>> _localVariables;
        private readonly Lazy<MethodDefinition> _method;

        internal LocalScope(LocalScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var localScope = Reader.GetLocalScope(metadataHandle);
            EndOffset = localScope.EndOffset;
            _importScope = new Lazy<ImportScope>(() => GetCodeElement<ImportScope>(localScope.ImportScope));
            Length = localScope.Length;
            _method = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(localScope.Method));
            StartOffset = localScope.StartOffset;
            _children = new Lazy<IReadOnlyList<LocalScope>>(LoadChildren);
            _localConstants = new Lazy<IReadOnlyList<LocalConstant>>(() => GetCodeElements<LocalConstant>(localScope.GetLocalConstants()));
            _localVariables = new Lazy<IReadOnlyList<LocalVariable>>(() => GetCodeElements<LocalVariable>(localScope.GetLocalVariables()));
        }

        public IReadOnlyList<LocalScope> Children => _children.Value;

        public int EndOffset { get; }

        public ImportScope ImportScope => _importScope.Value;

        public int Length { get; }

        public IReadOnlyList<LocalConstant> LocalConstants => _localConstants.Value;

        public IReadOnlyList<LocalVariable> LocalVariables => _localVariables.Value;

        public MethodDefinition Method => _method.Value;

        public int StartOffset { get; }

        private IReadOnlyList<LocalScope> LoadChildren()
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
