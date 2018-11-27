namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        ILEmitter Accept(StackEmitter visitor, ILEmitter il);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
