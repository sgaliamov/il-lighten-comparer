using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class ArrayItemVariable : IVariable
    {
        public ArrayItemVariable(
            Type arrayType,
            Type ownerType,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder indexVariable) {
            if (arrayType == null) { throw new ArgumentNullException(nameof(arrayType)); }

            IndexVariable = indexVariable ?? throw new ArgumentNullException(nameof(indexVariable));

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            GetItemMethod = arrayType.GetMethod(MethodName.Get, new[] { typeof(int) })
                            ?? throw new ArgumentException(nameof(arrayType));

            VariableType = arrayType.GetElementType();

            Arrays = new Dictionary<ushort, LocalBuilder>(2) {
                { Arg.X, xArray ?? throw new ArgumentNullException(nameof(xArray)) },
                { Arg.Y, yArray ?? throw new ArgumentNullException(nameof(yArray)) }
            };
        }

        public Dictionary<ushort, LocalBuilder> Arrays { get; }
        public MethodInfo GetItemMethod { get; }
        public LocalBuilder IndexVariable { get; }
        public Type VariableType { get; }
        public Type OwnerType { get; }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.Load(this, il, arg);

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.LoadAddress(this, il, arg);
    }
}
