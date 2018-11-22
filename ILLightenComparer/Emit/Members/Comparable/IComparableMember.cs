using System.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal interface IComparableMember : IMember
    {
        MethodInfo CompareToMethod { get; }
    }
}
