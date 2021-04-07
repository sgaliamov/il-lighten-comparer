using System.Reflection.Emit;

namespace Illuminator.Extensions
{
    public static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder) =>
            methodBuilder.GetILGenerator().UseIlluminator();

        public static ILEmitter CreateILEmitter(this ConstructorBuilder constructorBuilder) =>
            constructorBuilder.GetILGenerator().UseIlluminator();
    }
}