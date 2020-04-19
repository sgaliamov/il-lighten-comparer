using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityComparers
{

    internal interface IHashSeedSetter
    {
        void Set(long seed);
    }
}
