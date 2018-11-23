namespace ILLightenComparer.Tests.Visitor
{
    public interface IVisitor<in TAcceptor, TState>
    {
        TState Do(TAcceptor acceptor, TState state);
    }
}
