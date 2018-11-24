using System.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal interface IComparableMember : IMember
    {
        MethodInfo CompareToMethod { get; }
    }
}
