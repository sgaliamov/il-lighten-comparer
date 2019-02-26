using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ILLightenComparer.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();
        private readonly ConfigurationBuilder _configurations = new ConfigurationBuilder();
        private readonly ComparerContext _context;

        public ComparerBuilder()
        {
            _context = new ComparerContext(_configurations);
        }

        public IComparerBuilder DefineDefaultConfiguration(ComparerSettings settings)
        {
            _configurations.DefineDefaultConfiguration(settings);
            return this;
        }

        public IComparerBuilder DefineConfiguration(Type type, ComparerSettings settings)
        {
            _configurations.DefineConfiguration(type, settings);
            return this;
        }

        public IComparerBuilder SetComparer(Type type, IComparer comparer)
        {
            throw new NotImplementedException();
        }

        public IComparer<T> GetComparer<T>()
        {
            return (IComparer<T>)GetComparer(typeof(T));
        }

        public IComparer GetComparer(Type objectType)
        {
            return _comparers.GetOrAdd(
                objectType,
                key => _context.GetOrBuildComparerType(key).CreateInstance<IComparerContext, IComparer>(_context));
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        public IEqualityComparer GetEqualityComparer(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IComparerBuilder<T> For<T>()
        {
            return new GenericProxy<T>(this);
        }

        private sealed class GenericProxy<T> : IComparerBuilder<T>
        {
            private readonly ComparerBuilder _owner;

            public GenericProxy(ComparerBuilder comparerBuilder)
            {
                _owner = comparerBuilder;
            }

            public IComparerBuilder<T> SetComparer(IComparer<T> comparer)
            {
                throw new NotImplementedException();
            }

            public IComparerBuilder<TOther> For<TOther>()
            {
                return _owner.For<TOther>();
            }

            public IComparerBuilder<T> DefineConfiguration(ComparerSettings settings)
            {
                _owner.DefineConfiguration(typeof(T), settings);
                return this;
            }

            public IComparer<T> GetComparer()
            {
                return _owner.GetComparer<T>();
            }

            public IEqualityComparer<T> GetEqualityComparer()
            {
                return _owner.GetEqualityComparer<T>();
            }
        }
    }
}
