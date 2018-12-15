using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class VariableLoader
    {
        public ILEmitter Load(PropertyVariable variable, ILEmitter il, ushort arg)
        {
            if (variable.OwnerType.IsValueType)
            {
                il.LoadArgumentAddress(arg);
            }
            else
            {
                il.LoadArgument(arg);
            }

            return il.Call(variable.GetterMethod);
        }

        public ILEmitter LoadAddress(PropertyVariable variable, ILEmitter il, ushort arg)
        {
            return Load(variable, il, arg)
                   .Store(variable.VariableType.GetUnderlyingType(), out var local)
                   .LoadAddress(local);
        }

        public ILEmitter Load(FieldVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadArgument(arg)
                     .Emit(OpCodes.Ldfld, variable.FieldInfo);
        }

        public ILEmitter LoadAddress(FieldVariable variable, ILEmitter il, ushort arg)
        {
            if (variable.OwnerType.IsValueType)
            {
                il.LoadArgumentAddress(arg);
            }
            else
            {
                il.LoadArgument(arg);
            }

            return il.Emit(OpCodes.Ldflda, variable.FieldInfo);
        }
    }
}
