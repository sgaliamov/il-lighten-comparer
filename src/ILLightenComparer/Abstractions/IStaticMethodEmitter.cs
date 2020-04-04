using System;
using System.Reflection.Emit;

namespace ILLightenComparer.Abstractions
{
    internal interface IStaticMethodEmitter
    {
        void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder);
        bool NeedCreateCycleDetectionSets(Type objectType);
    }
}
