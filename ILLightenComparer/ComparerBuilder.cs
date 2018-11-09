using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit;
using ILLightenComparer.Emit.Types;

namespace ILLightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ComparerEmitter _comparerEmitter;
        private readonly EqualityComparerEmitter _equalityComparerEmitter;
        private readonly TypeEmitter _typeEmitter = new TypeEmitter();

        private CompareConfiguration _configuration = new CompareConfiguration();

        public ComparerBuilder()
        {
            _equalityComparerEmitter = new EqualityComparerEmitter(_typeEmitter);
            _comparerEmitter = new ComparerEmitter(_typeEmitter);
        }

        public IComparer<T> CreateComparer<T>() =>
            _comparerEmitter.Emit<T>(_configuration);

        public IComparer CreateComparer(Type type) =>
            _comparerEmitter.Emit(type, _configuration);

        public IEqualityComparer<T> CreateEqualityComparer<T>() =>
            _equalityComparerEmitter.Emit<T>(_configuration);

        public IEqualityComparer CreateEqualityComparer(Type type) =>
            _equalityComparerEmitter.Emit(type, _configuration);

        public ComparerBuilder SetConfiguration(CompareConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }
    }
}
