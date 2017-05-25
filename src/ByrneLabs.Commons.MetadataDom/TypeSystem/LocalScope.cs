using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class LocalScope : DebugCodeElement, ICodeElementWithTypedHandle<LocalScopeHandle, System.Reflection.Metadata.LocalScope>
    {
        private readonly Lazy<ImmutableArray<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<ImmutableArray<LocalConstant>> _localConstants;
        private readonly Lazy<ImmutableArray<LocalVariable>> _localVariables;
        private readonly Lazy<MethodDefinitionBase> _method;

        internal LocalScope(LocalScopeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetLocalScope(metadataHandle);
            EndOffset = RawMetadata.EndOffset;
            _importScope = MetadataState.GetLazyCodeElement<ImportScope>(RawMetadata.ImportScope);
            Length = RawMetadata.Length;
            _method = MetadataState.GetLazyCodeElement<MethodDefinitionBase>(RawMetadata.Method);
            StartOffset = RawMetadata.StartOffset;
            _children = new Lazy<ImmutableArray<LocalScope>>(LoadChildren);
            _localConstants = MetadataState.GetLazyCodeElements<LocalConstant>(RawMetadata.GetLocalConstants());
            _localVariables = MetadataState.GetLazyCodeElements<LocalVariable>(RawMetadata.GetLocalVariables());
        }
        public ImmutableArray<LocalScope> Children => _children.Value;
        public int EndOffset { get; }
        public ImportScope ImportScope => _importScope.Value;
        public int Length { get; }
        public ImmutableArray<LocalConstant> LocalConstants => _localConstants.Value;
        public ImmutableArray<LocalVariable> LocalVariables => _localVariables.Value;
        public MethodDefinitionBase Method => _method.Value;
        public int StartOffset { get; }

        public System.Reflection.Metadata.LocalScope RawMetadata { get; }

        public LocalScopeHandle MetadataHandle { get; }

        private ImmutableArray<LocalScope> LoadChildren()
        {
            var childrenHandles = new List<LocalScopeHandle>();
            var childrenEnumerator = RawMetadata.GetChildren();
            while (childrenEnumerator.MoveNext())
            {
                childrenHandles.Add(childrenEnumerator.Current);
            }

            return MetadataState.GetCodeElements<LocalScope>(childrenHandles);
        }
    }
}
