using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom
{
    public class SerializedType : TypeBase<SerializedType, string>
    {
        internal SerializedType(TypeBase<SerializedType, string> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
        }

        internal SerializedType(TypeBase<SerializedType, string> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
        }

        private Lazy<SerializedType> _declaringType;

        internal SerializedType(string assemblyQualifiedName, MetadataState metadataState) : base(assemblyQualifiedName, metadataState)
        {
            var nameParse = Regex.Match(assemblyQualifiedName, @"^((?:.+\.)?)([^\.]+)((?:\+.?)?), (.+?, Version=.+?, Culture=.+?, PublicKeyToken=.+?)$");
            if (!string.IsNullOrWhiteSpace(nameParse.Groups[3].Value))
            {
                var declaringTypeName = $"{nameParse.Groups[1].Value}.{nameParse.Groups[2].Value}, {nameParse.Groups[4].Value}";
                _declaringType = MetadataState.GetLazyCodeElement<SerializedType>(declaringTypeName);
                MemberType = MemberTypes.NestedType;
                Name = nameParse.Groups[3].Value;
            }
            else
            {
                _declaringType = new Lazy<SerializedType>(() => null);
                MemberType = MemberTypes.TypeInfo;
                Name = nameParse.Groups[2].Value;
            }
            Namespace = nameParse.Groups[1].Value.TrimEnd('.');
            var assemblyName = new AssemblyName(nameParse.Groups[4].Value);
            Assembly = MetadataState.FindAssemblyReference(assemblyName);
        }

        public override IAssembly Assembly { get; }

        public override bool IsGenericParameter { get; }

        public override string Namespace { get; }

        public override TypeBase DeclaringType => _declaringType.Value;

        public override MemberTypes MemberType { get; }

        public override string Name { get; }
    }
}
