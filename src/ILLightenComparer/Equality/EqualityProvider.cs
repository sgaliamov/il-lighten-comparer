using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Comparer
{
    internal sealed class EqualityProvider
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly GenericProvider _genericProvider;
        private readonly IConfigurationProvider _configurations;

        public EqualityProvider(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            var resolver = new ComparisonResolver(null, _configurations);
            _comparerTypeBuilder = new ComparerTypeBuilder(resolver, _configurations);
            _genericProvider = new GenericProvider(typeof(IEqualityComparer<>), (_) => null);
        }

        public MethodInfo GetStaticEqualsMethodInfo(Type type) =>
            _genericProvider.GetStaticMethodInfo(type, nameof(Equals));

        public MethodInfo GetCompiledStaticEqualsMethod(Type type) =>
            _genericProvider.GetCompiledStaticMethod(type, nameof(Equals));

        public MethodInfo GetStaticHashMethodInfo(Type type) =>
             _genericProvider.GetStaticMethodInfo(type, nameof(GetHashCode));

        public MethodInfo GetCompiledStaticHashMethod(Type type) =>
            _genericProvider.GetCompiledStaticMethod(type, nameof(GetHashCode));

    }
}
