using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IArgumentsMember : IAcceptor
    {
        bool LoadContext { get; }
    }
}
