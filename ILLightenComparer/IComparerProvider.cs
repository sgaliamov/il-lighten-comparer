using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    public interface IComparerProvider<in T> : IComparerProvider
    {
        IComparer<T> GetComparer();
        IEqualityComparer<T> GetEqualityComparer();
    }

    public interface IComparerProvider
    {
        IComparer GetComparer(Type objectType);
        IComparer<T> GetComparer<T>();
        IEqualityComparer GetEqualityComparer(Type objectType);
        IEqualityComparer<T> GetEqualityComparer<T>();
    }
}
