using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Emitters.Cases
{
    internal interface IComparableCase : IMember
    {
        MethodInfo CompareToMethod { get; }
    }
}
