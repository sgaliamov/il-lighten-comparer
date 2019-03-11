using System;
using System.Collections.Generic;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters;

namespace ILLightenComparer
{
    /// <summary>
    ///     Implements api to configure and get comparers.
    /// </summary>
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
        private readonly Context _context;

        public ComparerBuilder()
        {
            _context = new Context(_configurationBuilder);
        }

        public ComparerBuilder(Action<IConfigurationBuilder> config) : this()
        {
            Configure(config);
        }

        public IComparer<T> GetComparer<T>()
        {
            return _context.GetComparer<T>();
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            return _context.GetEqualityComparer<T>();
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

            public IComparer<TOther> GetComparer<TOther>()
            {
                return _subject.GetComparer<TOther>();
            }

            public IEqualityComparer<T> GetEqualityComparer()
            {
                return _subject.GetEqualityComparer<T>();
            }
        }
    }
}
