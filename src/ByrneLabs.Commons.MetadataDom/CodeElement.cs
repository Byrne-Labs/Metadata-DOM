using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class CodeElement
    {
        internal CodeElement(MetadataState metadataState)
        {
            MetadataState = metadataState;
        }

        protected abstract MetadataReader Reader { get; }

        internal MetadataState MetadataState { get; }

        protected Guid AsGuid(GuidHandle guidHandle) => Reader.GetGuid(guidHandle);

        protected string AsString(StringHandle stringHandle) => Reader.GetString(stringHandle);

        protected T GetCodeElement<T>(object handle) where T : CodeElement => MetadataState.GetCodeElement<T>(handle);

        protected CodeElement GetCodeElement(object handle) => MetadataState.GetCodeElement(handle);

        protected IReadOnlyList<T> GetCodeElements<T>(IEnumerable handles) where T : CodeElement => MetadataState.GetCodeElements<T>(handles);
    }
}
