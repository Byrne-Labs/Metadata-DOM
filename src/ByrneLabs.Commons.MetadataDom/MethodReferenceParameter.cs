using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodReferenceParameter : IParameter
    {
        internal MethodReferenceParameter(MethodReferenceBase methodReference, int position)
        {
            Position = position;
            ParameterType = methodReference.MethodSignature.Value.ParameterTypes[position - 1];
            Member = methodReference;
            IsOptional = position > methodReference.MethodSignature.Value.RequiredParameterCount;
        }

        public IEnumerable<CustomAttribute> CustomAttributes { get; } = new List<CustomAttribute>();

        public bool HasDefaultValue { get; }

        public bool IsIn { get; }

        public bool IsOptional { get; }

        public bool IsOut { get; }

        public bool IsRetval { get; } = false;

        public IMember Member { get; }

        public string Name { get; } = null;

        public TypeBase ParameterType { get; }

        public int Position { get; }

        public string TextSignature => ParameterType.FullName;
    }
}
