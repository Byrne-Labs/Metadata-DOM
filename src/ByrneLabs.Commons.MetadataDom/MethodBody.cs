using System;
using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodBody : CodeElementWithoutHandle
    {
        private readonly Lazy<IReadOnlyList<ExceptionRegion>> _exceptionRegions;
        private readonly Lazy<StandaloneSignature> _localSignature;

        internal MethodBody(int relativeVirtualAddress, MetadataState metadataState) : base(new HandlelessCodeElementKey<MethodBody>(relativeVirtualAddress), metadataState)
        {
            var methodBody = MetadataState.GetMethodBodyBlock(relativeVirtualAddress);
            _exceptionRegions = GetLazyCodeElements<ExceptionRegion>(methodBody.ExceptionRegions.Select(exceptionRegion => new HandlelessCodeElementKey<ExceptionRegion>(exceptionRegion)));
            _localSignature = GetLazyCodeElement<StandaloneSignature>(methodBody.LocalSignature);
            LocalVariablesInitialized = methodBody.LocalVariablesInitialized;
            MaxStack = methodBody.MaxStack;
            Size = methodBody.Size;
            methodBody.GetILBytes();
        }

        public IEnumerable<ExceptionRegion> ExceptionRegions => _exceptionRegions.Value;

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public bool LocalVariablesInitialized { get; }

        public int MaxStack { get; }

        public int Size { get; }

    }
}
