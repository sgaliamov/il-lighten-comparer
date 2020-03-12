using System.Reflection.Emit;

namespace ILLightenComparer.Shared
{
    internal interface IComparerStaticMethodEmitter
    {
        void Build(MethodBuilder staticMethodBuilder);
    }
}
