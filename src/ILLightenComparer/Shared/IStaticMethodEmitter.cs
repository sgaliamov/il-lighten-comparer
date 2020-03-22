using System;
using System.Reflection.Emit;

namespace ILLightenComparer.Shared
{
    internal interface IStaticMethodEmitter
    {
        void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder);
        bool NeedCreateCycleDetectionSets(Type objectType);
    }
}
