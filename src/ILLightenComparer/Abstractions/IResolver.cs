using ILLightenComparer.Variables;

namespace ILLightenComparer.Abstractions
{
    internal interface IResolver
    {
        IComparisonEmitter GetComparisonEmitter(IVariable variable);
    }
}
