namespace ILLightenComparer.Visitor
{
    internal interface IVisitor<in TAcceptor, TState>
    {
        TState Visit(TAcceptor acceptor, TState state);
    }
}
