using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember : IMember
    {
        MethodInfo GetterMethod { get; }
    }
}
