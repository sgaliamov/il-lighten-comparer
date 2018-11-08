using System;
using System.Collections;
using System.Collections.Generic;

namespace ILightenComparer
{
    public interface IComparerBuilder
    {
        IComparer CreateComparer(Type type);
        IComparer<T> CreateComparer<T>();
        IEqualityComparer CreateEqualityComparer(Type type);
        IEqualityComparer<T> CreateEqualityComparer<T>();
    }
}