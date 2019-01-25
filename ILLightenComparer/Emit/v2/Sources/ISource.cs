using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.v2.Sources
{
    internal interface ISource
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
