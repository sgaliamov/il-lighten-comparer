using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    public interface IComparersBuilder
    {
        IComparer CreateComparer(Type objectType);
        IComparer<T> CreateComparer<T>();
        IEqualityComparer CreateEqualityComparer(Type objectType);
        IEqualityComparer<T> CreateEqualityComparer<T>();
    }
}
