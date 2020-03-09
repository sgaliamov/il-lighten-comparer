using System;
using System.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Comparer
{
    internal sealed class EqualityMethodsProvider
    {
        private readonly GenericProvider _genericProvider;

        public EqualityMethodsProvider(GenericProvider genericProvider) => _genericProvider = genericProvider;

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
