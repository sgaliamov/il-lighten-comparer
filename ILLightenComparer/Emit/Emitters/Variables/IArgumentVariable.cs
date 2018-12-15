using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IArgumentVariable : IAcceptor
    {
        bool LoadContext { get; }
    }
}
