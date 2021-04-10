using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder) =>
            methodBuilder.GetILGenerator().UseIlluminator();

        public static ILEmitter CreateILEmitter(this ConstructorBuilder constructorBuilder) =>
            constructorBuilder.GetILGenerator().UseIlluminator();
    }
}