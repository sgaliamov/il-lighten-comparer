using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit;

namespace ILLightenComparer
{
    // todo: cache instances by type and configuration
    public sealed class ComparersBuilder : IComparersBuilder
    {
        private readonly ComparerBuilder _comparerBuilder;
        private readonly Context _context = new Context();
        private readonly EqualityComparerBuilder _equalityComparerBuilder;

        public ComparersBuilder()
        {
            _equalityComparerBuilder = new EqualityComparerBuilder(_context);
            _comparerBuilder = new ComparerBuilder(_context);
        }

        public IComparer<T> CreateComparer<T>() =>
            throw new NotImplementedException();

        public IComparer CreateComparer(Type type) =>
            _comparerBuilder.Build(type);

        public IEqualityComparer<T> CreateEqualityComparer<T>() =>
            _equalityComparerBuilder.Build<T>();

        public IEqualityComparer CreateEqualityComparer(Type type) =>
            _equalityComparerBuilder.Build(type);

        public ComparersBuilder SetConfiguration(CompareConfiguration configuration)
        {
            _context.SetConfiguration(configuration);
            return this;
        }
    }
}
