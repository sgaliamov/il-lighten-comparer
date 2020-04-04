using Illuminator;

namespace ILLightenComparer.Abstractions
{
    internal interface IHasherEmitter
    {
        ILEmitter Emit(ILEmitter il);
    }
}
