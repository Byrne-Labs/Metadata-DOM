using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison
{
    public static class ReflectionChecker
    {
        private static readonly Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<PropertyInfo, PropertyInfo>>> _propertiesToCompare = new Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<PropertyInfo, PropertyInfo>>>();

        public static string BaseDirectory { get; set; }

        private static DirectoryInfo PassedAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory, "../PassedTests"));

        private static DirectoryInfo ReadFailedAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory, "../ReadFailedTests"));

        private static DirectoryInfo TestAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory, "../TestAssemblies"));

        private static DirectoryInfo ValidationFailedAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory, "../ValidationFailedTests"));

        public static bool Check(FileInfo assemblyFile) => Check(assemblyFile, null);

        public static bool Check(FileInfo assemblyFile, FileInfo pdbFile)
        {
            var startTime = DateTime.Now;
            var originalAssemblyDirectory = assemblyFile.Directory;
            var errors = new List<string>();
            Exception readException = null;
            DirectoryInfo newFileDirectory;
            try
            {
                using (var metadata = pdbFile == null ? new Metadata(assemblyFile) : new Metadata(assemblyFile, pdbFile))
                {
                    try
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile.FullName);
                        var checkedMetadataElements = new List<CodeElement>();
                        var checkedReflectionElements = new List<object>();

                        foreach (var reflectionType in assembly.DefinedTypes)
                        {
                            var metadataType = metadata.TypeDefinitions.SingleOrDefault(codeElement => codeElement.FullName.Equals(reflectionType.FullName));
                            if (metadataType == null)
                            {
                                errors.Add($"Could not find type {reflectionType.FullName} with metadata");
                            }
                            else
                            {
                                CompareCodeElementsToReflectionData(metadataType, reflectionType, checkedMetadataElements, checkedReflectionElements, errors);
                            }
                        }
                        foreach (var module in assembly.Modules)
                        {
                            CompareCodeElementsToReflectionData(metadata.ModuleDefinition, module, checkedMetadataElements, checkedReflectionElements, errors);
                        }

                        var missingMembers = metadata.MemberDefinitions.Except(checkedMetadataElements.OfType<IMember>()).Select(member => $"Could not find member {member.FullName} with reflection").ToList();
                        errors.AddRange(missingMembers);
                    }
                    catch (Exception exception)
                    {
                        var realException = exception;
                        while (realException is TargetInvocationException && realException.InnerException != null)
                        {
                            realException = realException.InnerException;
                        }

                        errors.Add($"Assembly {assemblyFile.FullName} failed with exception:\r\n{realException}");
                        readException = readException ?? realException;
                    }

                    try
                    {
                        MetadataChecker.CheckMetadata(metadata);
                    }
                    catch (Exception exception)
                    {
                        readException = exception;
                        while (readException is TargetInvocationException && readException.InnerException != null)
                        {
                            readException = readException.InnerException;
                        }

                        errors.Add($"Assembly {assemblyFile.FullName} failed with exception:\r\n{readException}");
                    }
                }
            }
            catch (Exception exception)
            {
                readException = exception;
                errors.Add($"Assembly {assemblyFile.FullName} failed with exception:\r\n{exception}");
            }

            if (readException != null)
            {
                newFileDirectory = new DirectoryInfo(Path.Combine(ReadFailedAssemblyDirectory.FullName, readException.GetType().Name));
            }
            else if (errors.Any())
            {
                newFileDirectory = ValidationFailedAssemblyDirectory;
            }
            else
            {
                newFileDirectory = PassedAssemblyDirectory;
            }

            FileInfo newFileLocation;
            if (assemblyFile.FullName.ToLower().StartsWith(TestAssemblyDirectory.FullName.ToLower()))
            {
                newFileLocation = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), newFileDirectory.FullName));
                newFileLocation.Directory.Create();
                assemblyFile.MoveTo(newFileLocation.FullName);
            }
            else
            {
                newFileLocation = assemblyFile;
            }

            while (!originalAssemblyDirectory.GetFileSystemInfos().Any())
            {
                originalAssemblyDirectory.Delete();
                originalAssemblyDirectory = originalAssemblyDirectory.Parent;
            }

            var logFile = newFileLocation.FullName.Substring(0, newFileLocation.FullName.Length - 4) + ".log";
            var executionTime = DateTime.Now.Subtract(startTime);
            if (errors.Any())
            {
                errors.Add($"Analysis finished in {executionTime.TotalSeconds} seconds{Environment.NewLine}");
                File.WriteAllLines(logFile, errors);
            }
            else
            {
                File.WriteAllText(logFile, $"Analysis finished in {executionTime.TotalSeconds} seconds{Environment.NewLine}");
            }
            return !errors.Any();
        }

        private static void CompareCodeElementsToReflectionData(TypeDefinition metadataType, TypeInfo reflectionType, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataType, reflectionType, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            foreach (var reflectionMember in reflectionType.DeclaredMembers)
            {
                var reflectionMemberSignature = GetTextSignature(reflectionType, reflectionMember);
                var metadataMember = metadataType.Members.SingleOrDefault(codeElement => codeElement.GetType() == GetMetadataType(reflectionMember) && codeElement.TextSignature.Equals(reflectionMemberSignature));
                if (metadataMember == null)
                {
                    errors.Add($"Could not find member {reflectionMember.Name} on type {metadataType.FullName}");
                }
                else if (reflectionMember is TypeInfo)
                {
                    CompareCodeElementsToReflectionData((TypeDefinition)metadataMember, (TypeInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is PropertyInfo)
                {
                    CompareCodeElementsToReflectionData((PropertyDefinition)metadataMember, (PropertyInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is ConstructorInfo)
                {
                    CompareCodeElementsToReflectionData((ConstructorDefinition)metadataMember, (ConstructorInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is MethodInfo)
                {
                    CompareCodeElementsToReflectionData((MethodDefinition)metadataMember, (MethodInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is EventInfo)
                {
                    CompareCodeElementsToReflectionData((EventDefinition)metadataMember, (EventInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is FieldInfo)
                {
                    CompareCodeElementsToReflectionData((FieldDefinition)metadataMember, (FieldInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else
                {
                    throw new BadMetadataException($"Could not handle member type {reflectionMember.GetType().FullName}");
                }
            }
        }

        private static void CompareCodeElementsToReflectionData(PropertyDefinition metadataProperty, PropertyInfo reflectionProperty, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataProperty, reflectionProperty, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            CompareTypes($"Property {metadataProperty.FullName}", metadataProperty.PropertyType, reflectionProperty.PropertyType, errors);

            CompareElements(metadataProperty.FullName, metadataProperty, reflectionProperty, errors);
        }

        private static void CompareTypes(string sourceName, TypeBase metadataType, Type reflectionType, ICollection<string> errors)
        {
            if (!Equals(metadataType.Namespace, reflectionType.Namespace))
            {
                errors.Add($"{sourceName} reflection type namespace \"{reflectionType.Namespace}\" does not match metadata type namespace \"{metadataType.Namespace}\"");
            }
            if (!Equals(metadataType.Name, reflectionType.Name))
            {
                errors.Add($"{sourceName} reflection type name \"{reflectionType.Name}\" does not match metadata type name \"{metadataType.Name}\"");
            }
        }


        private static void CompareCodeElementsToReflectionData(MethodBase metadataMethodBase, System.Reflection.MethodBase reflectionMethodBase, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataMethodBase, reflectionMethodBase, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            if (reflectionMethodBase.GetParameters().Length != metadataMethodBase.Parameters.Count())
            {
                errors.Add($"{metadataMethodBase.FullName} has {reflectionMethodBase.GetParameters().Length} parameters in reflection but {metadataMethodBase.Parameters.Count()} in metadata");
            }

            foreach (var reflectionParameter in reflectionMethodBase.GetParameters())
            {
                var metadataParameter = metadataMethodBase.Parameters.SingleOrDefault(parameter => parameter.Position == reflectionParameter.Position);
                if (metadataParameter == null)
                {
                    errors.Add($"The parameter named {reflectionParameter.Name} with position {reflectionParameter.Position} on {metadataMethodBase.FullName} could not be found in metadata");
                    continue;
                }
                CompareTypes($"The parameter named {reflectionParameter.Name} with position {reflectionParameter.Position} on {metadataMethodBase.FullName}", metadataParameter.ParameterType, reflectionParameter.ParameterType, errors);
                CompareElements(metadataParameter.FullName, metadataParameter, reflectionParameter, errors);
            }

            CompareElements(metadataMethodBase.FullName, metadataMethodBase, reflectionMethodBase, errors);
        }

        private static void CompareCodeElementsToReflectionData(EventDefinition metadataEvent, EventInfo reflectionEvent, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataEvent, reflectionEvent, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            CompareElements(metadataEvent.FullName, metadataEvent, reflectionEvent, errors);
        }

        private static void CompareCodeElementsToReflectionData(FieldDefinition metadataField, FieldInfo reflectionField, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataField, reflectionField, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            CompareElements(metadataField.FullName, metadataField, reflectionField, errors);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for ModuleDefinition clases")]
        private static void CompareCodeElementsToReflectionData(ModuleDefinition metadataModule, Module reflectionModule, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataModule, reflectionModule, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }

            CompareElements("<module>", metadataModule, reflectionModule, errors);
        }

        private static void CompareElements(string elementName, object metadataElement, object reflectionElement, ICollection<string> errors)
        {
            foreach (var propertyToCompare in FindPropertiesToCompare(metadataElement.GetType(), reflectionElement.GetType()))
            {
                var metadataPropertyValue = propertyToCompare.Item1.GetValue(metadataElement);
                var reflectionPropertyValue = propertyToCompare.Item2.GetValue(reflectionElement);
                if (!Equals(metadataPropertyValue, reflectionPropertyValue))
                {
                    errors.Add($"{elementName}.{propertyToCompare.Item1.Name} has a value of {metadataPropertyValue} in metadata but a value of {reflectionPropertyValue} in reflection");
                }
            }
        }

        private static IEnumerable<Tuple<PropertyInfo, PropertyInfo>> FindPropertiesToCompare(Type metadataType, Type reflectionType)
        {
            var key = new Tuple<Type, Type>(metadataType, reflectionType);
            if (!_propertiesToCompare.ContainsKey(key))
            {
                var properties = new List<Tuple<PropertyInfo, PropertyInfo>>();
                var allProperties = metadataType.GetProperties().Select(property => property.Name).Intersect(reflectionType.GetProperties().Select(property => property.Name));
                foreach (var propertyName in allProperties)
                {
                    var metadataPropertyInfo = metadataType.GetProperty(propertyName);
                    var reflectionPropertyInfo = reflectionType.GetProperty(propertyName);
                    if (metadataPropertyInfo.PropertyType == reflectionPropertyInfo.PropertyType)
                    {
                        properties.Add(new Tuple<PropertyInfo, PropertyInfo>(metadataPropertyInfo, reflectionPropertyInfo));
                    }
                }

                _propertiesToCompare.Add(key, properties);
            }

            return _propertiesToCompare[key];
        }

        private static Type GetMetadataType(MemberInfo memberInfo)
        {
            Type metadataType;
            if (memberInfo is TypeInfo)
            {
                metadataType = typeof(TypeDefinition);
            }
            else if (memberInfo is PropertyInfo)
            {
                metadataType = typeof(PropertyDefinition);
            }
            else if (memberInfo is FieldInfo)
            {
                metadataType = typeof(FieldDefinition);
            }
            else if (memberInfo is EventInfo)
            {
                metadataType = typeof(EventDefinition);
            }
            else if (memberInfo is MethodInfo)
            {
                metadataType = typeof(MethodDefinition);
            }
            else if (memberInfo is ConstructorInfo)
            {
                metadataType = typeof(ConstructorDefinition);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return metadataType;
        }

        private static string GetTextSignature(TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            if (memberInfo is TypeInfo)
            {
                textSignature = GetTextSignature((TypeInfo)memberInfo);
            }
            else if (memberInfo is PropertyInfo)
            {
                textSignature = GetTextSignature(reflectedType, (PropertyInfo)memberInfo);
            }
            else if (memberInfo is FieldInfo)
            {
                textSignature = GetTextSignature(reflectedType, (FieldInfo)memberInfo);
            }
            else if (memberInfo is EventInfo)
            {
                textSignature = GetTextSignature(reflectedType, (EventInfo)memberInfo);
            }
            else if (memberInfo is MethodInfo)
            {
                textSignature = GetTextSignature(reflectedType, (MethodInfo)memberInfo);
            }
            else if (memberInfo is ConstructorInfo)
            {
                textSignature = GetTextSignature(reflectedType, (ConstructorInfo)memberInfo);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return textSignature;
        }

        private static string GetTextSignature(TypeInfo typeInfo) => typeInfo.FullName;

        private static string GetTextSignature(TypeInfo reflectedType, PropertyInfo propertyInfo) =>
            $"{reflectedType.FullName}.{propertyInfo.Name}" + ("Item".Equals(propertyInfo.Name) && propertyInfo.GetMethod.GetParameters().Any() ? $"[{string.Join(", ", propertyInfo.GetMethod.GetParameters().Select(parameter => parameter.ParameterType.Namespace + "." + parameter.ParameterType.Name))}]" : string.Empty);

        private static string GetTextSignature(TypeInfo reflectedType, FieldInfo fieldInfo) => $"{reflectedType.FullName}.{fieldInfo.Name}";

        private static string GetTextSignature(TypeInfo reflectedType, EventInfo eventInfo) => $"{reflectedType.FullName}.{eventInfo.Name}";

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for MethodInfo clases")]
        private static string GetTextSignature(TypeInfo reflectedType, MethodInfo methodInfo) => $"{reflectedType.FullName}.{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(parameter => GetTypeFullNameWithoutAssemblies(parameter.ParameterType.GetTypeInfo())))})";

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for ConstructorInfo clases")]
        private static string GetTextSignature(TypeInfo reflectedType, ConstructorInfo constructorInfo) => $"{reflectedType.FullName}({string.Join(", ", constructorInfo.GetParameters().Select(parameter => GetTypeFullNameWithoutAssemblies(parameter.ParameterType.GetTypeInfo())))})";

        private static string GetTypeFullNameWithoutAssemblies(TypeInfo type)
        {
            string parent;
            if (type.IsGenericParameter && type.DeclaringType != null || type.IsNested)
            {
                parent = GetTypeFullNameWithoutAssemblies(type.DeclaringType.GetTypeInfo()) + "+";
            }
            else
            {
                parent = string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace + ".";
            }
            var genericArgumentsText = type.GenericTypeArguments.Any() ? "[" + string.Join(",", type.GenericTypeArguments.Select(genericTypeArgument => $"[{GetTypeFullNameWithoutAssemblies(genericTypeArgument.GetTypeInfo())}]")) + "]" : string.Empty;

            var fullName = parent + type.Name + genericArgumentsText;

            return fullName;
        }

        private static bool WasChecked(CodeElement metadataElement, object reflectionElement, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements)
        {
            var wasChecked = checkedReflectionElements.Contains(reflectionElement);
            if (!wasChecked)
            {
                checkedMetadataElements.Add(metadataElement);
                checkedReflectionElements.Add(reflectionElement);
            }

            return wasChecked;
        }
    }
}
