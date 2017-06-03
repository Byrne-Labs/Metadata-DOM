using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using AssemblyToExpose = System.Reflection.Assembly;

#else
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public class SerializedType : EmptyTypeBase
    {
        private readonly Lazy<SerializedType> _declaringType;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This should only be used with SerializedType")]
        internal SerializedType(SerializedType baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<SerializedType>(baseType, typeElementModifier))
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This should only be used with SerializedType")]
        internal SerializedType(SerializedType genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<SerializedType>(genericTypeDefinition, genericTypeArguments))
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SerializedType(string assemblyQualifiedName, MetadataState metadataState) : base(new CodeElementKey<SerializedType>(assemblyQualifiedName), metadataState)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
            var nameParse = Regex.Match(assemblyQualifiedName, @"^((?:.+\.)?)([^\.]+)((?:\+.?)?), (.+?, Version=.+?, Culture=.+?, PublicKeyToken=.+?)$");
            if (!string.IsNullOrWhiteSpace(nameParse.Groups[3].Value))
            {
                var declaringTypeName = $"{nameParse.Groups[1].Value}.{nameParse.Groups[2].Value}, {nameParse.Groups[4].Value}";
                _declaringType = MetadataState.GetLazyCodeElement<SerializedType>(declaringTypeName);
                MemberType = MemberTypes.NestedType;
                UndecoratedName = nameParse.Groups[3].Value;
            }
            else
            {
                _declaringType = new Lazy<SerializedType>(() => null);
                MemberType = MemberTypes.TypeInfo;
                UndecoratedName = nameParse.Groups[2].Value;
            }
            Namespace = nameParse.Groups[1].Value.TrimEnd('.');
            if (string.IsNullOrEmpty( nameParse.Groups[4].Value))
            {
                Assembly = MetadataState.AssemblyDefinition;
            }
            else
            {
                var assemblyName = new AssemblyName(nameParse.Groups[4].Value);
                Assembly = MetadataState.FindAssemblyReference(assemblyName);
            }
        }

        public override AssemblyToExpose Assembly { get; }

        public override string AssemblyQualifiedName { get; }

        public override Type DeclaringType => _declaringType.Value;

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType { get; }

        public override string Namespace { get; }

        internal override string UndecoratedName { get; }
    }
}
