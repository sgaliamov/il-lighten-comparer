namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IMember
    {
        void Accept(StackEmitter stacker, ILEmitter il);
        void Accept(CompareEmitter emitter, ILEmitter il);
    }
}
