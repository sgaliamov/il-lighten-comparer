using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Variables
{
    internal sealed class EnumerableItemVariable : IVariable
    {
        private readonly IReadOnlyDictionary<ushort, LocalBuilder> _enumerators;
        private readonly MethodInfo _getCurrentMethod;

        public EnumerableItemVariable(Type ownerType, Type elementType, MethodInfo getCurrentMethod, IReadOnlyDictionary<ushort, LocalBuilder> enumerators)
        {
            OwnerType = ownerType;
            VariableType = elementType;

            _enumerators = enumerators;
            _getCurrentMethod = getCurrentMethod;
        }

        /// <summary>
        /// Element type.
        /// </summary>
        public Type VariableType { get; }

        /// <summary>
        /// Enumerator type.
        /// </summary>
        public Type OwnerType { get; }

        public ILEmitter Load(ILEmitter il, ushort arg) => il
            .Execute(Load(arg))
            .Call(_getCurrentMethod);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg) => il
            .Execute(Load(arg))
            .Call(_getCurrentMethod)
            .Store(VariableType, out var local)
            .LoadAddress(local);

        private Func<ILEmitter, ILEmitter> Load(ushort arg) => OwnerType.IsValueType
            ? Functional.LoadAddress(_enumerators[arg])
            : LoadLocal(_enumerators[arg]);
    }
}
