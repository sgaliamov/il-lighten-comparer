using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class NullableVariable : IVariable
    {
        public NullableVariable(Type ownerType, Type variableType, LocalBuilder x, LocalBuilder y)
        {
            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));
            GetValueMethod = variableType.GetPropertyGetter(MethodName.Value);

            Nullables = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, x },
                { Arg.Y, y }
            };
        }

        public MethodInfo GetValueMethod { get; set; }
        public Dictionary<ushort, LocalBuilder> Nullables { get; }
        public Type OwnerType { get; }
        public Type VariableType { get; }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.Load(this, il, arg);
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadAddress(this, il, arg);
        }
    }
}
