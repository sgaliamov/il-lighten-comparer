using System.Diagnostics;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder)
        {
            Debug.WriteLine($"Method {methodBuilder.DisplayName()}");
            return new ILEmitter(methodBuilder.GetILGenerator());
        }
    }
}
