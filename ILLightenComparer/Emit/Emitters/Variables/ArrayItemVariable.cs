using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class ArrayItemVariable : IVariable
    {
        public ArrayItemVariable(
            Type ownerType,
            Type variableType,
            MethodInfo itemMethod,
            LocalBuilder indexVariable)
        {
            OwnerType = ownerType;
            VariableType = variableType;
            GetItemMethod = itemMethod;
            IndexVariable = indexVariable;
        }

        public MethodInfo GetItemMethod { get; }
        public LocalBuilder IndexVariable { get; }

        /// <summary>
        ///     Where an array is defined.
        /// </summary>
        public Type OwnerType { get; }

        /// <summary>
        ///     Type of array element.
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
