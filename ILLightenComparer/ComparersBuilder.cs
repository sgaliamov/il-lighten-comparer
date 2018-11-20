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
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly Context _context = new Context();
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public ComparersBuilder()
        {
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(_context, _membersProvider);
            _comparerTypeBuilder = new ComparerTypeBuilder(_context, _membersProvider);
        }

        public IComparer<T> CreateComparer<T>()
        {
            var typeInfo = _comparerTypeBuilder.Build(typeof(T));

            return _context.CreateInstance<IComparer<T>>(typeInfo);
        }

        public IComparer CreateComparer(Type type)
        {
            var typeInfo = _comparerTypeBuilder.Build(type);

            return _context.CreateInstance<IComparer>(typeInfo);
        }

        public IEqualityComparer<T> CreateEqualityComparer<T>() =>
            _equalityComparerTypeBuilder.Build<T>();

        public IEqualityComparer CreateEqualityComparer(Type type) =>
            _equalityComparerTypeBuilder.Build(type);

        public ComparersBuilder SetConfiguration(CompareConfiguration configuration)
        {
            _context.SetConfiguration(configuration);
            return this;
        }
    }
}
