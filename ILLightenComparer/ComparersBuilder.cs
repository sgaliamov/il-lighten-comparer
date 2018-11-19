using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit;
using ILLightenComparer.Reflection;

namespace ILLightenComparer
{
    // todo: cache instances by type and configuration
    public sealed class ComparersBuilder : IComparersBuilder
    {
        private readonly ComparerBuilder _comparerBuilder;
        private readonly Context _context = new Context();
        private readonly EqualityComparerBuilder _equalityComparerBuilder;
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public ComparersBuilder()
        {
            _equalityComparerBuilder = new EqualityComparerBuilder(_context);
            _comparerBuilder = new ComparerBuilder(_context, _membersProvider);
        }

        public IComparer<T> CreateComparer<T>() =>
            _comparerBuilder.Build<T>();

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
