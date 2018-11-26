using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface INullableMember : IComparableMember, IPropertyMember
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
    }
}
