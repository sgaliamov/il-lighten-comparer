using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ILLightenComparer.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer
{
    // todo: cache instances by type and configuration
    public sealed class ComparersBuilder : IComparersBuilder
    {
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly TypeBuilderContext _context = new TypeBuilderContext();
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;

        public ComparersBuilder()
        {
            var membersProvider = new MembersProvider(_context);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(_context, membersProvider);
            _comparerTypeBuilder = new ComparerTypeBuilder(_context, membersProvider);
        }

        public IComparer<T> CreateComparer<T>() => (IComparer<T>)CreateComparer(typeof(T));

        public IComparer CreateComparer(Type objectType) =>
            _comparers.GetOrAdd(
                objectType,
                key => _comparerTypeBuilder.Build(key).CreateInstance<IComparer>());

        public IEqualityComparer<T> CreateEqualityComparer<T>() =>
            _equalityComparerTypeBuilder.Build<T>();

        public IEqualityComparer CreateEqualityComparer(Type objectType) =>
            _equalityComparerTypeBuilder.Build(objectType);

        public ComparersBuilder SetConfiguration(CompareConfiguration configuration)
        {
            _context.SetConfiguration(configuration);
            return this;
        }
    }
}
