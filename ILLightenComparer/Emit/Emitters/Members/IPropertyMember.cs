using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember : IVariable
    {
        MethodInfo GetterMethod { get; }
    }
}
