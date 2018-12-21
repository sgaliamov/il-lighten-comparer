using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class ArrayItemVariable : IVariable
    {
        public ArrayItemVariable(
            Type arrayMemberType,
            Type ownerType,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder indexVariable)
        {
            if (arrayMemberType == null) { throw new ArgumentNullException(nameof(arrayMemberType)); }

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            VariableType = arrayMemberType.GetElementType();

            IndexVariable = indexVariable ?? throw new ArgumentNullException(nameof(indexVariable));

            GetItemMethod = arrayMemberType.GetMethod(MethodName.Get, new[] { typeof(int) })
                            ?? throw new ArgumentException(nameof(arrayMemberType));

            Arrays = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, xArray ?? throw new ArgumentNullException(nameof(xArray)) },
                { Arg.Y, yArray ?? throw new ArgumentNullException(nameof(yArray)) }
            };
        }

        public Dictionary<ushort, LocalBuilder> Arrays { get; }
        public MethodInfo GetItemMethod { get; }
        public LocalBuilder IndexVariable { get; }
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
