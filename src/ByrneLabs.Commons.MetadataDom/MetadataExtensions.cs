using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public static class MetadataExtensions
    {
        public static IReadOnlyList<NamespaceDefinition> AllNamespaces(this Metadata metadata) =>
            metadata.TypeDefinitions.Where(typeDefinition => typeDefinition.NamespaceDefinition != null).SelectMany(typeDefinition => typeDefinition.NamespaceDefinition.Ancestors().Append(typeDefinition.NamespaceDefinition)).Distinct().ToList();

        public static IReadOnlyList<NamespaceDefinition> Ancestors(this NamespaceDefinition namespaceDefinition) =>
            namespaceDefinition.Parent == null ? new List<NamespaceDefinition>() : namespaceDefinition.Parent.Ancestors().Append(namespaceDefinition.Parent).ToList();

        public static IReadOnlyList<NamespaceDefinition> Namespaces(this Metadata metadata) =>
            metadata.AllNamespaces().Where(definedNamespace => definedNamespace.Parent == null).ToList();
    }
}
