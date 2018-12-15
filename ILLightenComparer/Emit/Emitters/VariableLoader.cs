using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class VariableLoader
    {
        public ILEmitter LoadMember(IPropertyVariable variable, ILEmitter il, ushort arg)
        {
            return LoadProperty(il, variable, arg);
        }

        public ILEmitter LoadMemberAddress(IPropertyVariable variable, ILEmitter il, ushort arg)
        {
            return LoadProperty(il, variable, arg)
                   .Store(variable.VariableType.GetUnderlyingType(), out var local)
                   .LoadAddress(local);
        }

        public ILEmitter LoadMember(IFieldVariable variable, ILEmitter il, ushort arg)
        {
            return LoadField(il, variable, arg);
        }

        public ILEmitter LoadMemberAddress(IFieldVariable variable, ILEmitter il, ushort arg)
        {
            return LoadFieldAddress(il, variable, arg);
        }

        private static ILEmitter LoadProperty(ILEmitter il, IPropertyVariable variable, ushort argumentIndex)
        {
            if (variable.DeclaringType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Call(variable.GetterMethod);
        }

        private static ILEmitter LoadField(ILEmitter il, IFieldVariable variable, ushort argumentIndex)
        {
            return il.LoadArgument(argumentIndex)
                     .Emit(OpCodes.Ldfld, variable.FieldInfo);
        }

        private static ILEmitter LoadFieldAddress(ILEmitter il, IFieldVariable variable, ushort argumentIndex)
        {
            if (variable.DeclaringType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Emit(OpCodes.Ldflda, variable.FieldInfo);
        }
    }
}
