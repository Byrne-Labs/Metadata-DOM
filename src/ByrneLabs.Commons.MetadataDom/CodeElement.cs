using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class CodeElement
    {
        internal CodeElement(object key, MetadataState metadataState)
        {
            MetadataState = metadataState;
            metadataState.CacheCodeElement(this, key);
        }

        protected abstract MetadataReader Reader { get; }

        internal MetadataState MetadataState { get; }

        protected Guid AsGuid(GuidHandle guidHandle) => Reader.GetGuid(guidHandle);

        protected string AsString(StringHandle stringHandle) => Reader.GetString(stringHandle);

        internal IEnumerable<T> GetCodeElementsWithHandle<T>(IEnumerable handles) where T : CodeElement => handles.Cast<object>().Select(GetCodeElementWithHandle<T>).ToList();

        internal IEnumerable<T> GetCodeElementsWithoutHandle<T>(IEnumerable keys) where T : CodeElement => keys.Cast<object>().Select(GetCodeElementWithoutHandle<T>).ToList();

        internal T GetCodeElementWithHandle<T>(object handle) where T : CodeElement => (T) MetadataState.GetCodeElement(handle);

        internal CodeElement GetCodeElementWithHandle(object handle) => MetadataState.GetCodeElement(handle);

        internal T GetCodeElementWithoutHandle<T>(object key) where T : CodeElement => (T) MetadataState.GetCodeElement(new HandlelessCodeElementKey<T>(key));

        internal Lazy<IEnumerable<T>> GetLazyCodeElementsWithHandle<T>(IEnumerable handles) where T : CodeElement => new Lazy<IEnumerable<T>>(() => handles.Cast<object>().Select(GetCodeElementWithHandle<T>).ToList());

        internal Lazy<IEnumerable<T>> GetLazyCodeElementsWithoutHandle<T>(IEnumerable keys) where T : CodeElement => new Lazy<IEnumerable<T>>(() => keys.Cast<object>().Select(GetCodeElementWithoutHandle<T>).ToList());

        internal Lazy<CodeElement> GetLazyCodeElementWithHandle(object handle) => new Lazy<CodeElement>(() => GetCodeElementWithHandle(handle));

        internal Lazy<T> GetLazyCodeElementWithHandle<T>(object handle) where T : CodeElement => new Lazy<T>(() => GetCodeElementWithHandle<T>(handle));

        internal Lazy<T> GetLazyCodeElementWithoutHandle<T>(object key) where T : CodeElement => new Lazy<T>(() => (T) MetadataState.GetCodeElement(new HandlelessCodeElementKey<T>(key)));
    }
}
