namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        ILEmitter LoadArguments(StackEmitter visitor, ILEmitter il);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
