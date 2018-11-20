using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder) =>
            new ILEmitter(methodBuilder.GetILGenerator());
    }
}
