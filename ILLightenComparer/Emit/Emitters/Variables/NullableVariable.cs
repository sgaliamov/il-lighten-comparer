﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class NullableVariable : IVariable
    {
        public NullableVariable(Type variableType, Type ownerType, LocalBuilder x, LocalBuilder y)
        {
            if (variableType == null) { throw new ArgumentNullException(nameof(variableType)); }

            if (!variableType.IsNullable()) { throw new ArgumentException(nameof(variableType)); }

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            VariableType = variableType.GetUnderlyingType();

            Nullables = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, x ?? throw new ArgumentNullException(nameof(x)) },
                { Arg.Y, y ?? throw new ArgumentNullException(nameof(y)) }
            };

            GetValueMethod = variableType.GetPropertyGetter(MethodName.Value);
        }

        public MethodInfo GetValueMethod { get; set; }
        public Dictionary<ushort, LocalBuilder> Nullables { get; }
        public Type OwnerType { get; }

        /// <summary>
        ///     Underlying type of Nullable.
        /// </summary>
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
