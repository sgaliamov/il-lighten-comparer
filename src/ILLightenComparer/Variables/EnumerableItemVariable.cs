using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Variables
{
    internal sealed class EnumerableItemVariable : IVariable
    {
        private readonly Dictionary<ushort, LocalBuilder> _enumerators;
        private readonly MethodInfo _getCurrentMethod;

        public EnumerableItemVariable(Type ownerType, LocalBuilder xEnumerator, LocalBuilder yEnumerator)
        {
            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            _enumerators = new Dictionary<ushort, LocalBuilder>(2) {
                { Arg.X, xEnumerator ?? throw new ArgumentNullException(nameof(xEnumerator)) },
                { Arg.Y, yEnumerator ?? throw new ArgumentNullException(nameof(yEnumerator)) }
            };

            if (yEnumerator.LocalType != xEnumerator.LocalType) {
                throw new ArgumentException($"Enumerator types are not matched: {xEnumerator}, {yEnumerator}.");
            }

            var enumeratorType = xEnumerator.LocalType;
            if (!enumeratorType.ImplementsGeneric(typeof(IEnumerator<>))) {
                throw new ArgumentException($"Unexpected type {enumeratorType}.", nameof(enumeratorType));
            }

            VariableType = enumeratorType?.GetGenericArguments().SingleOrDefault()
                           ?? throw new ArgumentException(nameof(enumeratorType));

            _getCurrentMethod = enumeratorType.GetPropertyGetter(nameof(IEnumerator.Current));
        }

        public Type VariableType { get; }
        public Type OwnerType { get; }

        public ILEmitter Load(ILEmitter il, ushort arg) =>
            il.LoadLocal(_enumerators[arg])
              .Call(_getCurrentMethod);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg) =>
             il.LoadLocal(_enumerators[arg])
               .Call(_getCurrentMethod)
               .Store(VariableType, out var local)
               .LoadAddress(local);
    }
}
