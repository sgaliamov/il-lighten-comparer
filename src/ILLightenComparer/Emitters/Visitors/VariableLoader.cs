﻿using ILLightenComparer.Emitters.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Visitors
{
    internal sealed class VariableLoader
    {
        public ILEmitter Load(PropertyMemberVariable variable, ILEmitter il, ushort arg)
        {
            if (variable.OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            }
            else {
                il.LoadArgument(arg);
            }

            return il.Call(variable.GetterMethod);
        }

        public ILEmitter LoadAddress(PropertyMemberVariable variable, ILEmitter il, ushort arg) =>
            Load(variable, il, arg)
                .Store(variable.VariableType.GetUnderlyingType(), out var local)
                .LoadAddress(local);

        public ILEmitter Load(FieldMemberVariable variable, ILEmitter il, ushort arg) =>
            il.LoadArgument(arg)
              .LoadField(variable.FieldInfo);

        public ILEmitter LoadAddress(FieldMemberVariable variable, ILEmitter il, ushort arg)
        {
            if (variable.OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            }
            else {
                il.LoadArgument(arg);
            }

            return il.LoadFieldAddress(variable.FieldInfo);
        }

        public ILEmitter Load(ArrayItemVariable variable, ILEmitter il, ushort arg) =>
            il.LoadLocal(variable.Arrays[arg])
              .LoadLocal(variable.IndexVariable)
              .Call(variable.GetItemMethod);

        public ILEmitter LoadAddress(ArrayItemVariable variable, ILEmitter il, ushort arg) =>
            il.LoadLocal(variable.Arrays[arg])
              .LoadLocal(variable.IndexVariable)
              .Call(variable.GetItemMethod)
              .Store(variable.VariableType, out var local)
              .LoadAddress(local);

        public ILEmitter Load(EnumerableItemVariable variable, ILEmitter il, ushort arg) =>
            il.LoadLocal(variable.Enumerators[arg])
              .Call(variable.GetCurrentMethod);

        public ILEmitter LoadAddress(EnumerableItemVariable variable, ILEmitter il, ushort arg) =>
            il.LoadLocal(variable.Enumerators[arg])
              .Call(variable.GetCurrentMethod)
              .Store(variable.VariableType, out var local)
              .LoadAddress(local);

        public ILEmitter Load(NullableVariable variable, ILEmitter il, ushort arg) =>
            il.LoadAddress(variable.Nullables[arg])
              .Call(variable.GetValueMethod);

        public ILEmitter LoadAddress(NullableVariable variable, ILEmitter il, ushort arg)
        {
            var underlyingType = variable.VariableType.GetUnderlyingType();

            return il.LoadAddress(variable.Nullables[arg])
                     .Call(variable.GetValueMethod)
                     .Store(underlyingType, out var x)
                     .LoadAddress(x);
        }

        public ILEmitter Load(ILEmitter il, ushort arg) => il.LoadArgument(arg);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg) => il.LoadArgumentAddress(arg);
    }
}