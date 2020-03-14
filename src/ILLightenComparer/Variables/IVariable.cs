using System;
using Illuminator;

namespace ILLightenComparer.Variables
{
    internal interface IVariable
    {
        Type OwnerType { get; }
        Type VariableType { get; }

        ILEmitter Load(ILEmitter il, ushort arg);
        ILEmitter LoadAddress(ILEmitter il, ushort arg);
    }

    internal static class VariableExtensions
    {
        public static Action<ILEmitter> Load(this IVariable variable, ushort arg) =>
          (ILEmitter il) => variable.Load(il, arg);

        public static Action<ILEmitter> LoadAddress(this IVariable variable, ushort arg) =>
            (ILEmitter il) => variable.LoadAddress(il, arg);
    }
}
