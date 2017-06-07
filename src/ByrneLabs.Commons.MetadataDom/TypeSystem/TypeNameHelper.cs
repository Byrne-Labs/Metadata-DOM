using System;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public static class TypeNameExtensions
    {
        internal static string GetFullName(this Type type) => GetFullName((TypeBase) type.GetTypeInfo());

        internal static string GetFullName(this TypeBase type) => type.GetFullName(GetFullName, true);

        internal static string GetFullNameWithoutAssemblies(this Type type) => GetFullNameWithoutAssemblies((TypeBase) type.GetTypeInfo());

        internal static string GetFullNameWithoutAssemblies(this TypeBase type) => type.GetFullName(GetFullNameWithoutAssemblies, true);

        internal static string GetFullNameWithoutGenericArguments(this Type type) => GetFullNameWithoutGenericArguments((TypeBase) type.GetTypeInfo());

        internal static string GetFullNameWithoutGenericArguments(this TypeBase type) => type.GetFullName(GetFullNameWithoutGenericArguments, false);

        internal static string GetNameModifiers(this TypeBase type) => (type.IsThisArray ? "[]" : string.Empty) + new string('*', type.PointerCount) + (type.IsThisByRef ? "&" : string.Empty);

        private static string GetFullName(this Type type, Func<Type, string> nameGetter, bool includeGenericArguments) => GetFullName((TypeBase) type.GetTypeInfo(), nameGetter, includeGenericArguments);

        private static string GetFullName(this TypeBase type, Func<Type, string> nameGetter, bool includeGenericArguments)
        {
#if DEBUG
            lock (_recursionCheckSyncRoot)
            {
                _recursionCount++;
                if (_recursionCount > 100)
                {
                    var message = $"Aborted before likely stack overflow with {_recursionCount} levels of recursion";
                    _recursionCount = 0;
                    throw new InvalidOperationException(message);
                }
            }
#endif
            string fullName;
            var name = type.Name;
            if (type.IsGenericParameter && type.DeclaringType == null)
            {
                fullName = type.Name;
            }
            else
            {
                var typeToUse = type.TypeElementModifier != null && type.UnmodifiedType != null ? (TypeBase) type.UnmodifiedType.GetTypeInfo() : type;
                string parent;
                if (typeToUse.IsNested && typeToUse.DeclaringType.GetTypeInfo().IsGenericType)
                {
                    parent = typeToUse.DeclaringType.GetFullName(nameGetter, includeGenericArguments) + "+";
                }
                else if (typeToUse.IsGenericParameter && typeToUse.DeclaringType != null || typeToUse.IsNested)
                {
                    parent = $"{nameGetter(typeToUse.DeclaringType)}+";
                }
                else
                {
                    parent = string.Empty;
                }
                string @namespace;
                if (typeToUse.IsNested && !typeToUse.IsGenericParameter && typeToUse.MetadataNamespace?.StartsWith("<") == true && typeToUse.Name.Contains(">"))
                {
                    @namespace = typeToUse.MetadataNamespace + ".";
                }
                else if (string.IsNullOrEmpty(typeToUse.Namespace) || typeToUse.IsNested && !typeToUse.IsGenericParameter)
                {
                    @namespace = string.Empty;
                }
                else
                {
                    @namespace = typeToUse.Namespace + ".";
                }
                string genericArgumentsText;
                if (includeGenericArguments && typeToUse.HasGenericTypeArguments && !typeToUse.GenericTypeArguments.SequenceEqual(typeToUse.GenericTypeParameters))
                {
                    if (typeToUse.IsDelegate)
                    {
                        genericArgumentsText = "[" + string.Join(",", typeToUse.GenericTypeArguments.Select(genericTypeArgument => genericTypeArgument.Name)) + "]";
                    }
                    else
                    {
                        genericArgumentsText = "[" + string.Join(",", typeToUse.GenericTypeArguments.Select(genericTypeArgument => genericTypeArgument.GetFullName(GetFullNameWithoutAssemblies, genericTypeArgument.DeclaringType != typeToUse) ?? genericTypeArgument.Name)) + "]";
                    }
                }

                else
                {
                    genericArgumentsText = string.Empty;
                }

                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (typeToUse == type)
                {
                    fullName = parent + @namespace + typeToUse.UndecoratedName + genericArgumentsText + typeToUse.GetNameModifiers();
                }
                else
                {
                    fullName = typeToUse.FullNameWithoutAssemblies + type.GetNameModifiers();
                }
            }
#if DEBUG
            lock (_recursionCheckSyncRoot)
            {
                _recursionCount--;
            }
#endif

            return fullName;
        }

#if DEBUG
        /*
         * This method has been prone to stack overflows which are very, very hard to debug.  This makes it a lot simpler. -- Jonathan Byrne 06/06/2017
         */
        [ThreadStatic]
        private static int _recursionCount;
        private static readonly object _recursionCheckSyncRoot = new object();
#endif
    }
}
