using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class Module : System.Reflection.Module
    {
        internal const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public abstract Guid BaseGenerationId { get; }

        public abstract int Generation { get; }

        public abstract Guid GenerationId { get; }

        public abstract bool Manifest { get; }

        public override int MDStreamVersion => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.FieldInfo[] GetFields(BindingFlags bindingFlags) => GetFieldsImpl(bindingFlags);

        public override System.Reflection.MethodInfo[] GetMethods(BindingFlags bindingFlags) => GetMethodsImpl(bindingFlags);

        public override string ToString() => Name;

        protected abstract System.Reflection.FieldInfo[] GetAllFields();

        protected abstract System.Reflection.MethodInfo[] GetAllMethods();

        private System.Reflection.FieldInfo[] GetFieldsImpl(BindingFlags bindingFlags) => GetAllFields().OfType<FieldInfo>().Where(field => (field.BindingFlags & bindingFlags) == bindingFlags).ToArray<System.Reflection.FieldInfo>();

        private System.Reflection.MethodInfo[] GetMethodsImpl(BindingFlags bindingFlags) => GetAllMethods().Cast<MethodInfo>().Where(method => (method.BindingFlags & bindingFlags) == bindingFlags).ToArray<System.Reflection.MethodInfo>();
    }
}
