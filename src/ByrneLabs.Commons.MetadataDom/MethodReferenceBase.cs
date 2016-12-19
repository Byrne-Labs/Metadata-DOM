using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class MethodReferenceBase : MemberReferenceBase, IMethodBase
    {
        internal MethodReferenceBase(MemberReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var parameters = new List<IParameter>();
            for (var position = 1; position <= MethodSignature.Value.ParameterTypes.Length; position++)
            {
                parameters.Add(new MethodReferenceParameter(this, position));
            }

            Parameters = parameters;
        }

        public TypeBase DeclaringType => (TypeBase) Parent;

        public abstract string FullName { get; }

        public MemberTypes MemberType => IsConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public abstract string TextSignature { get; }

        public bool ContainsGenericParameters { get; } = false;

        public IEnumerable<TypeBase> GenericArguments { get; } = new List<TypeBase>();

        public bool IsConstructor => this is IConstructor;

        public bool IsGenericMethod { get; } = false;

        public IEnumerable<IParameter> Parameters { get; }
    }
}
