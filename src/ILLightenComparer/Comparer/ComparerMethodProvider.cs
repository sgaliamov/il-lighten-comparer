using System;
using System.Reflection;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparerMethodProvider
    {
        private readonly GenericProvider _genericProvider;

        public ComparerMethodProvider(GenericProvider genericProvider) => _genericProvider = genericProvider;

        public MethodInfo GetStaticCompareMethodInfo(Type type) =>
            _genericProvider.GetStaticMethodInfo(type, MethodName.Compare);

        public MethodInfo GetCompiledStaticCompareMethod(Type type) =>
            _genericProvider.GetCompiledStaticMethod(type, MethodName.Compare);
    }
}
