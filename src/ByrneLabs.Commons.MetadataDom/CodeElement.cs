using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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

        protected CodeElement GetCodeElement(object handle) => MetadataState.GetCodeElement(handle);

        protected Lazy<CodeElement> GetLazyCodeElement(object handle) => new Lazy<CodeElement>(() => MetadataState.GetCodeElement(handle));

        internal T GetCodeElement<T>(object handle) where T : ICodeElementWithHandle => (T) (object) MetadataState.GetCodeElement(handle);

        internal T GetCodeElement<T>(HandlelessCodeElementKey key) where T : ICodeElementWithoutHandle => (T) (object) MetadataState.GetCodeElement(key);

        internal IReadOnlyList<T> GetCodeElements<T>(IEnumerable handles) where T : ICodeElementWithHandle => handles.Cast<object>().Select(handle => MetadataState.GetCodeElement(handle)).Cast<T>().ToList();

        internal IReadOnlyList<T> GetCodeElements<T>(IEnumerable<HandlelessCodeElementKey> keys) where T : ICodeElementWithoutHandle => keys.Select(handle => MetadataState.GetCodeElement(handle)).Cast<T>().ToList();

        internal Lazy<T> GetLazyCodeElement<T>(object handle) where T : ICodeElementWithHandle => new Lazy<T>(() => (T) (object) MetadataState.GetCodeElement(handle));

        internal Lazy<T> GetLazyCodeElement<T>(HandlelessCodeElementKey key) where T : ICodeElementWithoutHandle => new Lazy<T>(() => (T) (object) MetadataState.GetCodeElement(key));

        internal Lazy<IReadOnlyList<T>> GetLazyCodeElements<T>(IEnumerable handles) where T : ICodeElementWithHandle => new Lazy<IReadOnlyList<T>>(() => handles.Cast<object>().Select(handle => MetadataState.GetCodeElement(handle)).Cast<T>().ToList());

        internal Lazy<IReadOnlyList<T>> GetLazyCodeElements<T>(IEnumerable<HandlelessCodeElementKey> keys) where T : ICodeElementWithoutHandle => new Lazy<IReadOnlyList<T>>(() => keys.Select(handle => MetadataState.GetCodeElement(handle)).Cast<T>().ToList());
    }
}
