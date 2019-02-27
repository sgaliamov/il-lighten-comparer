using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Config;

namespace ILLightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
        private readonly ComparerContext _context;

        public ComparerBuilder()
        {
            _context = new ComparerContext(_configurationBuilder);
        }

        public IComparer<T> GetComparer<T>()
        {
            return _context.GetComparer<T>();
        }

        public IComparer GetComparer(Type objectType)
        {
            return null;
            //return _context.GetComparer(objectType);
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
            return new Proxy<T>(this);
        }

        public IComparerBuilder Configure(Action<IConfigurationBuilder> config)
        {
            config(_configurationBuilder);

            return this;
        }

        private sealed class Proxy<T> : IComparerBuilder<T>
        {
            private readonly ComparerBuilder _subject;

            public Proxy(ComparerBuilder subject)
            {
                _subject = subject;
            }

            public IComparerBuilder<TOther> For<TOther>()
            {
                return _subject.For<TOther>();
            }

            public IComparerBuilder<T> Configure(Action<IConfigurationBuilder<T>> config)
            {
                _subject._configurationBuilder.Configure(config);

                return this;
            }

            public IComparer<T> GetComparer()
            {
                return _subject.GetComparer<T>();
            }

            public IEqualityComparer<T> GetEqualityComparer()
            {
                return _subject.GetEqualityComparer<T>();
            }
        }
    }
}
