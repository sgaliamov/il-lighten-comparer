using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Variables
{
    internal sealed class NullableVariables : IVariable
    {
        private readonly MethodInfo _getValueMethod;
        private readonly IReadOnlyDictionary<ushort, LocalBuilder> _nullables;

        public NullableVariables(Type variableType, Type ownerType, IReadOnlyDictionary<ushort, LocalBuilder> nullables)
        {
            Debug.Assert(variableType.IsNullable());

            OwnerType = ownerType;
            VariableType = variableType.GetUnderlyingType();

            _nullables = nullables;
            _getValueMethod = variableType.GetPropertyGetter("Value");
        }

        public Type VariableType { get; }
        public Type OwnerType { get; }

        public ILEmitter Load(ILEmitter il, ushort arg) => il
            .Ldloca(_nullables[arg])
            .Call(_getValueMethod);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            var underlyingType = VariableType.GetUnderlyingType();

            return il.Ldloca(_nullables[arg])
                     .Call(_getValueMethod)
                     .Stloc(underlyingType, out var x)
                     .Ldloca(x);
        }
    }
}
