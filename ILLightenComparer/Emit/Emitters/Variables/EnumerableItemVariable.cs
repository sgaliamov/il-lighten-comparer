using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class EnumerableItemVariable : IVariable
    {
        private EnumerableItemVariable(Type enumeratorType, Type ownerType)
        {
            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            VariableType = enumeratorType?.GetGenericArguments().FirstOrDefault()
                           ?? throw new ArgumentException(nameof(enumeratorType));

            GetCurrentMethod = enumeratorType.GetPropertyGetter(MethodName.Current);
        }

        public MethodInfo GetCurrentMethod { get; }
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

        public static IVariable Create(Type enumeratorType, Type ownerType)
        {
            return enumeratorType.ImplementsGeneric(typeof(IEnumerator<>))
                       ? new EnumerableItemVariable(enumeratorType, ownerType)
                       : null;
        }
    }
}
