using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.v2.Sources
{
    /// <summary>
    ///     Arguments of the compare function.
    /// </summary>
    internal sealed class Arguments : ISource
    {
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }
    }
}
