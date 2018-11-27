using System.Reflection;
using ILLightenComparer.Emit.Emitters.Behavioural;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface INullableMember : IComparableMember, IPropertyMember
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
    }
}
