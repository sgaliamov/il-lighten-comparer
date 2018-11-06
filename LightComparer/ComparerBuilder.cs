using System;
using System.Collections;
using System.Collections.Generic;

namespace LightComparer
{
    public sealed class ComparerBuilder
    {
        private readonly Configuration _configuration;

        public ComparerBuilder(Configuration configuration) => _configuration = configuration;

        public IComparer<T> CreateComparer<T>() => throw new NotImplementedException();
        public IComparer CreateComparer(Type type) => throw new NotImplementedException();
        public IEqualityComparer<T> CreateEqualityComparer<T>() => throw new NotImplementedException();
        public IEqualityComparer CreateEqualityComparer(Type type) => throw new NotImplementedException();
    }
}
