using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal interface IComparableMember : IMember
    {
        MethodInfo CompareToMethod { get; }
    }
}
