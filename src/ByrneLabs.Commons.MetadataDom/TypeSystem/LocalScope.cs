using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class LocalScope : MetadataDom.LocalScope, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<IEnumerable<LocalConstant>> _localConstants;
        private readonly Lazy<IEnumerable<LocalVariable>> _localVariables;
        private readonly Lazy<MethodBase> _method;

        internal LocalScope(LocalScopeHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<LocalScope>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            MetadataState = metadataState;
            RawMetadata = MetadataState.PdbReader.GetLocalScope(metadataHandle);
            EndOffset = RawMetadata.EndOffset;
            _importScope = MetadataState.GetLazyCodeElement<ImportScope>(RawMetadata.ImportScope);
            Length = RawMetadata.Length;
            _method = MetadataState.GetLazyCodeElement<MethodBase>(RawMetadata.Method);
            StartOffset = RawMetadata.StartOffset;
            _children = new Lazy<IEnumerable<LocalScope>>(LoadChildren);
            _localConstants = MetadataState.GetLazyCodeElements<LocalConstant>(RawMetadata.GetLocalConstants());
            _localVariables = MetadataState.GetLazyCodeElements<LocalVariable>(RawMetadata.GetLocalVariables());
        }

        public override IEnumerable<MetadataDom.LocalScope> Children => _children.Value;

        public override int EndOffset { get; }

        public override MetadataDom.ImportScope ImportScope => _importScope.Value;

        public override int Length { get; }

        public override IEnumerable<LocalConstantInfo> LocalConstants => _localConstants.Value;

        public override IEnumerable<LocalVariableInfo> LocalVariables => _localVariables.Value;

        public LocalScopeHandle MetadataHandle { get; }

        public override MethodBase Method => _method.Value;

        public System.Reflection.Metadata.LocalScope RawMetadata { get; }

        public override int StartOffset { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        private IEnumerable<LocalScope> LoadChildren()
        {
            var childrenHandles = new List<LocalScopeHandle>();
            var childrenEnumerator = RawMetadata.GetChildren();
            while (childrenEnumerator.MoveNext())
            {
                childrenHandles.Add(childrenEnumerator.Current);
            }

            return MetadataState.GetCodeElements<LocalScope>(childrenHandles).ToImmutableArray();
        }
    }
}
