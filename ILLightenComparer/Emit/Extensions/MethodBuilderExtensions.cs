using System.Diagnostics;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder)
        {
            Debug.WriteLine($"\n{methodBuilder.DisplayName()}");
            return new ILEmitter(methodBuilder.GetILGenerator());
        }
    }
}
