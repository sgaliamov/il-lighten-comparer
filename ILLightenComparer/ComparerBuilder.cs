using System;
using System.Collections.Generic;
using System.Threading;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters;
using ILLightenComparer.Extensions;

namespace ILLightenComparer
{
    /// <summary>
    ///     Implements api to configure and get comparers.
    /// </summary>
    public sealed class ComparerBuilder : IComparerBuilder
    {
        private readonly ConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
        private Lazy<Context> _context;

        public ComparerBuilder()
        {
            InitContext();
        }

        public ComparerBuilder(Action<IConfigurationBuilder> config) : this()
        {
            Configure(config);
        }

        public IComparer<T> GetComparer<T>()
        {
            return _context.Value.GetComparer<T>();
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

            // todo: test - define configuration, get comparer, change configuration, get comparer, they should be different
            InitContext(); 

            return this;
        }

        public IComparerBuilder SetCustomComparer<T>(IComparer<T> instance)
        {
            _configurationBuilder.SetCustomComparer(typeof(T), instance);

            InitContext();

            return this;
        }

        public IComparerBuilder SetCustomComparer<TComparer>()
        {
            var genericType = typeof(TComparer);
            var genericInterface = genericType.GetGenericInterface(typeof(IComparer<>));
            if (genericInterface == null)
            {
                throw new ArgumentException($"{nameof(TComparer)} is not generic {typeof(IComparer<>)}");
            }

            var type = genericInterface.GenericTypeArguments[0];
            var comparer = genericType.Create<TComparer>();

            _configurationBuilder.SetCustomComparer(type, comparer);

            InitContext();

            return this;
        }

        private void InitContext()
        {
            _context = new Lazy<Context>(() => new Context(_configurationBuilder), LazyThreadSafetyMode.PublicationOnly);
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

                _subject.InitContext(); // todo: test

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

            //public IEqualityComparer<T> GetEqualityComparer()
            //{
            //    return _subject.GetEqualityComparer<T>();
            //}
        }
    }
}
