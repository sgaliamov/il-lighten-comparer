namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        ILEmitter Accept(StackEmitter stacker, ILEmitter il);
        ILEmitter Accept(CompareEmitter emitter, ILEmitter il);
    }
}
