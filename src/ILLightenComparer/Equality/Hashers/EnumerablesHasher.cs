using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class EnumerablesHasher : IHasherEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IVariable _variable;
        private readonly HasherResolver _resolver;
        private readonly Type _enumeratorType;
        private readonly Type _elementType;
        private readonly MethodInfo _getEnumeratorMethod;
        private readonly MethodInfo _moveNextMethod;
        private readonly MethodInfo _getCurrentMethod;

        private EnumerablesHasher(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            _resolver = resolver;
            _configuration = configuration;
            _variable = variable;

            var variableType = variable.VariableType;

            _elementType = variableType
               .FindGenericInterface(typeof(IEnumerable<>))
               .GetGenericArguments()
               .Single();

            _getEnumeratorMethod = variableType.FindMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes);
            _enumeratorType = _getEnumeratorMethod.ReturnType;
            _moveNextMethod = _enumeratorType.FindMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
            _getCurrentMethod = _enumeratorType.GetPropertyGetter(nameof(IEnumerator.Current));
        }

        public static EnumerablesHasher Create(HasherResolver resolver, IConfigurationProvider configuration, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray) {
                return new EnumerablesHasher(resolver, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            var config = _configuration.Get(_variable.OwnerType);

            return il
                .LoadLong(config.HashSeed)
                .Store(typeof(long), out var hash)
                .Execute(this.Emit(hash));
        }

        public ILEmitter Emit(ILEmitter il, LocalBuilder hash)
        {
            // todo: 2.
            //if (_configuration.Get(_variable.OwnerType).IgnoreCollectionOrder) {
            //    return EmitCompareAsSortedArrays(il, gotoNext, x, y);
            //}

            il.Execute(_variable.Load(Arg.Input))
              .Store(_variable.VariableType, out var enumerable)
              .Call(_getEnumeratorMethod, LoadLocal(enumerable))
              .Store(_enumeratorType, out var xEnumerator);

            return il;
        }
    }
}
