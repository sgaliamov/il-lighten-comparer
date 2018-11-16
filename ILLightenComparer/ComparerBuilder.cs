using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit;

namespace ILLightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ComparerEmitter _comparerEmitter;
        private readonly Context _context = new Context();
        private readonly EqualityComparerEmitter _equalityComparerEmitter;

        public ComparerBuilder()
        {
            _equalityComparerEmitter = new EqualityComparerEmitter(_context);
            _comparerEmitter = new ComparerEmitter(_context);
        }

        public IComparer<T> CreateComparer<T>() =>
            _comparerEmitter.Emit<T>();

        public IComparer CreateComparer(Type type) =>
            _comparerEmitter.Emit(type);

        public IEqualityComparer<T> CreateEqualityComparer<T>() =>
            _equalityComparerEmitter.Emit<T>();

        public IEqualityComparer CreateEqualityComparer(Type type) =>
            _equalityComparerEmitter.Emit(type);

        public ComparerBuilder SetConfiguration(CompareConfiguration configuration)
        {
            _context.SetConfiguration(configuration);
            return this;
        }
    }
}
