using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class EnumerableItemVariable : IVariable
    {
        public EnumerableItemVariable(Type ownerType, LocalBuilder xEnumerator, LocalBuilder yEnumerator)
        {
            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            Enumerators = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, xEnumerator ?? throw new ArgumentNullException(nameof(xEnumerator)) },
                { Arg.Y, yEnumerator ?? throw new ArgumentNullException(nameof(yEnumerator)) }
            };

            if (yEnumerator.LocalType != xEnumerator.LocalType)
            {
                throw new ArgumentException($"Enumerator types are not matched: {xEnumerator}, {yEnumerator}.");
            }

            var enumeratorType = xEnumerator.LocalType;
            if (!enumeratorType.ImplementsGeneric(typeof(IEnumerator<>)))
            {
                throw new ArgumentException($"Unexpected type {enumeratorType}.", nameof(enumeratorType));
            }

            VariableType = enumeratorType?.GetGenericArguments().SingleOrDefault()
                           ?? throw new ArgumentException(nameof(enumeratorType));

            GetCurrentMethod = enumeratorType.GetPropertyGetter(MethodName.Current);
        }

        public Dictionary<ushort, LocalBuilder> Enumerators { get; }
        public MethodInfo GetCurrentMethod { get; }
        public Type VariableType { get; }
        public Type OwnerType { get; }

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
