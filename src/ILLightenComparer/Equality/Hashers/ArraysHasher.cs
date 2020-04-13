using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Equality;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ArraysHasher : IHasherEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IVariable _variable;
        private readonly HasherResolver _resolver;

        private ArraysHasher(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            _resolver = resolver;
            _configuration = configuration;
            _variable = variable;
        }

        public static ArraysHasher Create(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysHasher(resolver, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            var arrayType = _variable.VariableType;
            var ownerType = _variable.OwnerType;
            var config = _configuration.Get(ownerType);

            il.LoadLong(config.HashSeed) // start hash
              .Store(typeof(long), out var hash)
              .Execute(_variable.Load(Arg.Input)) // load array
              .Store(arrayType, out var array)
              .LoadInteger(0) // start loop
              .Store(typeof(int), out var index)
              .EmitArrayLength(arrayType, array, out var count)
              .DefineLabel(out var loopStart);

            using (il.LocalsScope()) {
                il.MarkLabel(loopStart)
                  .IfNotEqual_Un_S(LoadLocal(index), LoadLocal(count), out var next)
                  .Return(hash)
                  .MarkLabel(next);
            }

            using (il.LocalsScope()) {
                var itemVariable = new ArrayItemVariable(arrayType, ownerType, array, array, index); // todo: 2. refactor ArrayItemVariable
                var itemHasher = _resolver.GetHasherEmitter(itemVariable);

                il.EmitHashing(hash, itemHasher.Emit).GoTo(loopStart);
            }

            return il;
        }
    }
}
