using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class ArrayHashEmitter
    {
        private readonly IVariable _variable;
        private readonly HasherResolver _resolver;

        public ArrayHashEmitter(HasherResolver resolver, IVariable variable)
        {
            _resolver = resolver;
            _variable = variable;
        }

        public ILEmitter Emit(ILEmitter il, LocalBuilder array, LocalBuilder hash)
        {
            var arrayType = _variable.VariableType;

            il.LoadInteger(0) // start loop
              .Store(typeof(int), out var index)
              .EmitArrayLength(arrayType, array, out var count)
              .DefineLabel(out var loopStart)
              .DefineLabel(out var loopEnd);

            using (il.LocalsScope()) {
                il.MarkLabel(loopStart)
                  .IfNotEqual_Un_S(LoadLocal(index), LoadLocal(count), out var next)
                  .GoTo(loopEnd)
                  .MarkLabel(next);
            }

            using (il.LocalsScope()) {
                var arrays = new Dictionary<ushort, LocalBuilder>(1) { [Arg.Input] = array };
                var itemVariable = new ArrayItemVariable(arrayType, _variable.OwnerType, arrays, index);

                _resolver
                    .GetHasherEmitter(itemVariable)
                    .EmitHashing(il, hash)
                    .Add(LoadLocal(index), LoadInteger(1))
                    .Store(index)
                    .GoTo(loopStart);
            }

            return il.MarkLabel(loopEnd).LoadLocal(hash);
        }
    }
}
