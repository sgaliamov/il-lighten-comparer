using System;
using System.Reflection.Emit;

namespace ILLightenComparer.Shared
{
    internal interface IComparerStaticMethodEmitter
    {
        void Build(Type objectType, MethodBuilder staticMethodBuilder);
        bool IsCreateCycleDetectionSets(Type objectType);
    }
}
