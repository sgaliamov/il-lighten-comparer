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

        public ComparerBuilder(Action<IConfigurationBuilder> config) : this()
        {
            Configure(config);
        }

        public IComparer<T> GetComparer<T>()
        {
            return _context.GetComparer<T>();
        }

        public IComparer GetComparer(Type objectType)
        {
            return _context.GetComparer(objectType);
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            return _context.GetEqualityComparer<T>();
        }

        public IEqualityComparer GetEqualityComparer(Type objectType)
        {
            return _context.GetEqualityComparer(objectType);
        }

        public IComparerBuilder<T> For<T>()
        {
            return new Proxy<T>(this);
        }

        public IComparerBuilder<T> For<T>(Action<IConfigurationBuilder<T>> config)
        {
            return new Proxy<T>(this).Configure(config);
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

            public IComparerBuilder<TOther> For<TOther>(Action<IConfigurationBuilder<TOther>> config)
            {
                return _subject.For(config);
            }

            public IComparerBuilder<T> Configure(Action<IConfigurationBuilder<T>> config)
            {
                _subject._configurationBuilder.Configure(config);

                return this;
            }

            public IComparerBuilder Builder => _subject;

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
