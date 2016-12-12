using System;
using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public class GenericTypeInstantiation : TypeBase
    {
        internal GenericTypeInstantiation(TypeBase genericTypeDefinition, IEnumerable<TypeBase> typeArguments, MetadataState metadataState) : base(CreateKey(genericTypeDefinition, typeArguments), metadataState)
        {
            GenericTypeDefinition = genericTypeDefinition;
            TypeArguments = typeArguments;
        }

        public override string AssemblyQualifiedName => GenericTypeDefinition.AssemblyQualifiedName;

        public override string FullName { get; }

        public override bool IsAbstract { get; }

        public override bool IsArray => false;
        public override TypeBase ElementType => null;

        public override bool IsClass { get; }

        public override bool IsEnum => false;

        public override bool IsGenericParameter { get; }

        public override bool IsGenericType => true;

        public override bool IsGenericTypeDefinition => false;

        public override bool IsImport { get; }

        public override bool IsInterface { get; }

        public override bool IsNestedAssembly { get; }

        public override bool IsNestedFamANDAssem { get; }

        public override bool IsNestedFamily { get; }

        public override bool IsNestedFamORAssem { get; }

        public override bool IsNestedPrivate { get; }

        public override bool IsNestedPublic { get; }

        public override bool IsNotPublic { get; }

        public override bool IsPrimitive => false;

        public override bool IsPublic { get; }

        public override string Name { get; }

        public override string Namespace => GenericTypeDefinition.Namespace;

        public IEnumerable<TypeBase> TypeArguments { get; }

        protected override TypeBase GenericTypeDefinition { get; }


        internal static HandlelessCodeElementKey CreateKey(TypeBase genericTypeDefinition, IEnumerable<TypeBase> typeArguments) => new HandlelessCodeElementKey<GenericTypeInstantiation>(genericTypeDefinition, typeArguments);
    }
}
