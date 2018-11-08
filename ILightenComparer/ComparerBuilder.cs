using System;
using System.Collections;
using System.Collections.Generic;
using ILightenComparer.Emit;

namespace ILightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ComparerEmitter _comparerEmitter = new ComparerEmitter();
        private readonly EqualityComparerEmitter _equalityComparerEmitter = new EqualityComparerEmitter();
        private CompareConfiguration _configuration;

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
