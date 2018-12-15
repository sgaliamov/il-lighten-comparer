using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IPropertyVariable : IVariable
    {
        MethodInfo GetterMethod { get; }
    }
}
