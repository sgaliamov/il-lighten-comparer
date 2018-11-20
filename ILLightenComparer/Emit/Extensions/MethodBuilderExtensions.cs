using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static ILEmitter GetILEmitter(this MethodBuilder methodBuilder) =>
            new ILEmitter(methodBuilder.GetILGenerator());
    }
}
