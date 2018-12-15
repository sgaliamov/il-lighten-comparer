using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IArgumentsVariable : IAcceptor
    {
        bool LoadContext { get; }
    }
}
