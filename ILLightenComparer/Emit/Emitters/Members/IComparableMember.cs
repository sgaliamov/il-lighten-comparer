using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IComparableMember : IMember
    {
        MethodInfo CompareToMethod { get; }
    }
}
