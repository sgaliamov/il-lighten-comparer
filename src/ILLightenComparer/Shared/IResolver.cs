using ILLightenComparer.Variables;

namespace ILLightenComparer.Shared
{
    internal interface IResolver
    {
        IComparisonEmitter GetComparisonEmitter(IVariable variable);
    }
}
