using Illuminator;

namespace ILLightenComparer.Shared
{
    internal interface IHasherEmitter
    {
        ILEmitter Emit(ILEmitter il);
    }
}
