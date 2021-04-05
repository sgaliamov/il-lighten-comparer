using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.FunctionalExtensions;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class ArrayHasher : IHasherEmitter
    {
        private readonly IVariable _variable;
        private readonly bool _hasCustomComparer;
        private readonly Configuration _configuration;
        private readonly ArrayHashEmitter _arrayHashEmitter;

        private ArrayHasher(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            _variable = variable;
            _arrayHashEmitter = new ArrayHashEmitter(resolver, variable);
            _configuration = configuration.Get(variable.OwnerType);
            _hasCustomComparer = configuration.HasCustomComparer(variable.VariableType.GetElementType());
        }

        public static ArrayHasher Create(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArrayHasher(resolver, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il) => il
            .Ldc_I8(_configuration.HashSeed)
            .Stloc(typeof(long), out var hash)
            .Emit(this.Emit(hash));

        public ILEmitter Emit(ILEmitter il, LocalBuilder hash)
        {
            var arrayType = _variable.VariableType;

            il.Emit(_variable.Load(Arg.Input)) // load array
              .Stloc(arrayType, out var array)
              .IfTrue_S(LoadLocal(array), out var begin)
              .LoadInteger(0)
              .GoTo(out var end)
              .MarkLabel(begin);

            if (_configuration.IgnoreCollectionOrder) {
                il.EmitArraySorting(_hasCustomComparer, arrayType.GetElementType(), array);
            }

            return _arrayHashEmitter.Emit(il, arrayType, array, hash).MarkLabel(end);
        }
    }
}
