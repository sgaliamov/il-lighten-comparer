using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Emitters.Behavioural
{
    internal interface IPropertyMember : IMember
    {
        MethodInfo GetterMethod { get; }
    }
}
