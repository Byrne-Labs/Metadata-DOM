using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public IEnumerable<LocalScope> Children => _children.Value;

        public int EndOffset { get; }

        public ImportScope ImportScope => _importScope.Value;

        public int Length { get; }

        public IEnumerable<LocalConstant> LocalConstants => _localConstants.Value;

        public IEnumerable<LocalVariable> LocalVariables => _localVariables.Value;

        public MethodDefinitionBase Method => _method.Value;

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
