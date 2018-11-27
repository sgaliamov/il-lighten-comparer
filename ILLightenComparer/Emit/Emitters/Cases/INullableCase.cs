using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Emitters.Cases
{
    internal interface INullableCase : IMember
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
        MethodInfo CompareToMethod { get; }
    }
}
