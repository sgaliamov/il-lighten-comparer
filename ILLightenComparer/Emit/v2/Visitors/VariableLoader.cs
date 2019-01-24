using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Variables.Collections;
using ILLightenComparer.Emit.v2.Variables.Members;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class VariableLoader
    {
        public ILEmitter Load(PropertyMemberVariable variable, ILEmitter il, ushort arg)
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

        public ILEmitter LoadAddress(PropertyMemberVariable variable, ILEmitter il, ushort arg)
        {
            return Load(variable, il, arg)
                   .Store(variable.VariableType.GetUnderlyingType(), out var local)
                   .LoadAddress(local);
        }

        public ILEmitter Load(FieldMemberVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadArgument(arg)
                     .Emit(OpCodes.Ldfld, variable.FieldInfo);
        }

        public ILEmitter LoadAddress(FieldMemberVariable variable, ILEmitter il, ushort arg)
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

        public ILEmitter Load(ArrayItemVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadLocal(variable.Arrays[arg])
                     .LoadLocal(variable.IndexVariable)
                     .Call(variable.GetItemMethod);
        }

        public ILEmitter LoadAddress(ArrayItemVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadLocal(variable.Arrays[arg])
                     .LoadLocal(variable.IndexVariable)
                     .Call(variable.GetItemMethod)
                     .Store(variable.VariableType, out var local)
                     .LoadAddress(local);
        }

        public ILEmitter Load(EnumerableItemVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadLocal(variable.Enumerators[arg])
                     .Call(variable.GetCurrentMethod);
        }

        public ILEmitter LoadAddress(EnumerableItemVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadLocal(variable.Enumerators[arg])
                     .Call(variable.GetCurrentMethod)
                     .Store(variable.VariableType, out var local)
                     .LoadAddress(local);
        }

        public ILEmitter Load(NullableVariable variable, ILEmitter il, ushort arg)
        {
            return il.LoadAddress(variable.Nullables[arg])
                     .Call(variable.GetValueMethod);
        }

        public ILEmitter LoadAddress(NullableVariable variable, ILEmitter il, ushort arg)
        {
            var underlyingType = variable.VariableType.GetUnderlyingType();

            return il.LoadAddress(variable.Nullables[arg])
                     .Call(variable.GetValueMethod)
                     .Store(underlyingType, out var x)
                     .LoadAddress(x);
        }

        public ILEmitter Load(ArgumentComparison variable, ILEmitter il, ushort arg)
        {
            return il.LoadArgument(arg);
        }

        public ILEmitter LoadAddress(ArgumentComparison variable, ILEmitter il, ushort arg)
        {
            return il.LoadArgumentAddress(arg);
        }
    }
}
