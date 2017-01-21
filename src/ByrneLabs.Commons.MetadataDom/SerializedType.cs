using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom
{
    public class SerializedType : TypeBase<SerializedType, string>
    {
        private readonly Lazy<SerializedType> _declaringType;

        internal SerializedType(TypeBase<SerializedType, string> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
        }

        internal SerializedType(TypeBase<SerializedType, string> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
        }

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

        internal override string UndecoratedName { get; }
    }
}
