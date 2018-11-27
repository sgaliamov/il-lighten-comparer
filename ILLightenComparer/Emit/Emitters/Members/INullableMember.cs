using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface INullableMember : IMember
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
        MethodInfo CompareToMethod { get; }
    }
}
