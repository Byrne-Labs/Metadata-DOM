using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom
{
    public class SerializedType : TypeBase<SerializedType, string>
    {
        private readonly Lazy<SerializedType> _declaringType;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SerializedType(TypeBase<SerializedType, string> baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SerializedType(TypeBase<SerializedType, string> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal SerializedType(string assemblyQualifiedName, MetadataState metadataState) : base(assemblyQualifiedName, metadataState)
        {
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
            var assemblyName = new AssemblyName(nameParse.Groups[4].Value);
            Assembly = MetadataState.FindAssemblyReference(assemblyName);
        }

        public override IAssembly Assembly { get; }

        public override ImmutableArray<CustomAttribute> CustomAttributes { get; } = ImmutableArray<CustomAttribute>.Empty;

        public override TypeBase DeclaringType => _declaringType.Value;

        public override bool IsGenericParameter => false;

        public override MemberTypes MemberType { get; }

        public override string Namespace { get; }

        protected override string MetadataNamespace { get; } = null;

        internal override string UndecoratedName { get; }
    }
}
