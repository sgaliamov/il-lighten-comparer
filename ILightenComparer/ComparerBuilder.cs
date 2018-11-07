using System;
using System.Collections;
using System.Collections.Generic;

namespace ILightenComparer
{
    public sealed class ComparerBuilder
    {
        private readonly CompareConfiguration _configuration;

        public ComparerBuilder(CompareConfiguration configuration) => _configuration = configuration;

        public IComparer<T> CreateComparer<T>() => throw new NotImplementedException();
        public IComparer CreateComparer(Type type) => throw new NotImplementedException();
        public IEqualityComparer<T> CreateEqualityComparer<T>() => throw new NotImplementedException();
        public IEqualityComparer CreateEqualityComparer(Type type) => throw new NotImplementedException();
    }
}
