using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class ArrayItemVariable : IVariable
    {
        private ArrayItemVariable(Type arrayMemberType, LocalBuilder indexVariable)
        {
            OwnerType = arrayMemberType.DeclaringType;
            VariableType = arrayMemberType.GetElementType();
            GetItemMethod = arrayMemberType.GetMethod(MethodName.ArrayGet, new[] { typeof(int) });
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

        public static IVariable Create(Type arrayMemberType, LocalBuilder indexVariable)
        {
            return arrayMemberType.IsArray && arrayMemberType.GetArrayRank() == 1
                       ? new ArrayItemVariable(arrayMemberType, indexVariable)
                       : null;
        }
    }
}
