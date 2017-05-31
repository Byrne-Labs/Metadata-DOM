using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using MethodBaseToExpose = System.Reflection.MethodBase;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class LocalScope : MetadataDom.LocalScope, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<LocalScope>> _children;
        private readonly Lazy<ImportScope> _importScope;
        private readonly Lazy<ImmutableArray<LocalConstant>> _localConstants;
        private readonly Lazy<ImmutableArray<LocalVariable>> _localVariables;
        private readonly Lazy<MethodBaseToExpose> _method;

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
            _method = MetadataState.GetLazyCodeElement<MethodBaseToExpose>(RawMetadata.Method);
            StartOffset = RawMetadata.StartOffset;
            _children = new Lazy<ImmutableArray<LocalScope>>(LoadChildren);
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

        public override MethodBaseToExpose Method => _method.Value;

        public System.Reflection.Metadata.LocalScope RawMetadata { get; }

        public override int StartOffset { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

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
