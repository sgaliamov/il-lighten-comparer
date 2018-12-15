using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyVariable : IVariable
    {
        MethodInfo GetterMethod { get; }
    }
}
