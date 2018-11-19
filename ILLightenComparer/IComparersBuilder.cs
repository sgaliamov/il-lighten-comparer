using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    public interface IComparersBuilder
    {
        IComparer CreateComparer(Type type);
        IComparer<T> CreateComparer<T>();
        IEqualityComparer CreateEqualityComparer(Type type);
        IEqualityComparer<T> CreateEqualityComparer<T>();
    }
}
