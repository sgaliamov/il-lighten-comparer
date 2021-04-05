using System;
using System.Reflection.Emit;

namespace Illuminator.Extensions
{
    public static class MethodBuilderExtensions
    {
        public static ILEmitter CreateILEmitter(this MethodBuilder methodBuilder) =>
            methodBuilder.GetILGenerator().UseIlluminator();

        public static ILEmitter CreateILEmitter(this ConstructorBuilder constructorBuilder) =>
            constructorBuilder.GetILGenerator().UseIlluminator();

        public static ILEmitter Cast<T>(this ILEmitter self, ILEmitterFunc value) => value(self).Cast(typeof(T));

        // todo: 3. test
        public static ILEmitter Cast(this ILEmitter self, Type type) => Type.GetTypeCode(type) switch {
            TypeCode.Int64 => self.Conv_I8(),
            TypeCode.Int32 => self.Conv_I4(),
            _ => type.IsValueType
                ? self.Unbox_Any()
                : self.Castclass(type)
        };
    }
}
