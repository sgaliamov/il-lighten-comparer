using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IArgumentsVariable : IAcceptor
    {
        bool LoadContext { get; }
    }
}
