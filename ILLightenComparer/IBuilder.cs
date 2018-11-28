using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    public interface IBuilder
    {
        IComparer GetComparer(Type objectType);
        IComparer<T> GetComparer<T>();
        IEqualityComparer GetEqualityComparer(Type objectType);
        IEqualityComparer<T> GetEqualityComparer<T>();
    }
}
