/*
   ___ ___ _  _ ___ ___    _ _____ ___ ___     ___ ___  ___  ___
  / __| __| \| | __| _ \  /_\_   _| __|   \   / __/ _ \|   \| __|
 | (_ | _|| .` | _||   / / _ \| | | _|| |) | | (_| (_) | |) | _|
  \___|___|_|\_|___|_|_\/_/ \_\_| |___|___/   \___\___/|___/|___|

*/

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Illuminator
{
    public static class FunctionalExtensions
    {
        public static ILEmitterFunc CallMethod(MethodInfo methodInfo, params ILEmitterFunc[] parameters) => il => il.CallMethod(methodInfo, parameters);
        public static ILEmitterFunc CallMethod(MethodInfo methodInfo) => il => il.CallMethod(methodInfo);
        public static ILEmitterFunc CallMethod(ConstructorInfo constructor) => il => il.Call(constructor);
        public static ILEmitterFunc Cast(Type type) => il => il.Cast(type);
        public static ILEmitterFunc Cast<T>(ILEmitterFunc value) => il => il.Cast<T>(value);
        public static ILEmitterFunc Constrained(Type type) => il => il.Constrained(type);
        public static ILEmitterFunc EmitIf(bool condition, params ILEmitterFunc[] actions) => il => il.ExecuteIf(condition, actions);
        public static ILEmitterFunc Greater_S(ILEmitterFunc a, ILEmitterFunc b, Label label) => il => il.Greater_S(a, b, label);
        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue, ILEmitterFunc elseAction) => il => il.If(action, whenTrue, elseAction);
        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue) => il => il.If(action, whenTrue);
        public static ILEmitterFunc If_S(ILEmitterFunc action, ILEmitterFunc whenTrue, ILEmitterFunc elseAction) => il => il.If_S(action, whenTrue, elseAction);
        public static ILEmitterFunc If_S(ILEmitterFunc action, ILEmitterFunc whenTrue) => il => il.If_S(action, whenTrue);

        public static ILEmitterFunc LessOrEqual_S(ILEmitterFunc a, ILEmitterFunc b, Label label) => il => il.LessOrEqual_S(a, b, label);
        public static ILEmitterFunc LoadAddress(LocalVariableInfo local) => il => il.LoadAddress(local);
        public static ILEmitterFunc LoadArgument(ushort argumentIndex) => il => il.LoadArgument(argumentIndex);
        public static ILEmitterFunc LoadArgumentAddress(ushort argumentIndex) => il => il.LoadArgumentAddress(argumentIndex);
        public static ILEmitterFunc LoadCaller(LocalVariableInfo local) => il => il.LoadCaller(local);
        public static ILEmitterFunc LoadLocal(LocalVariableInfo local) => il => il.LoadLocal(local);
        public static ILEmitterFunc LoadLocal(int localIndex) => il => il.LoadLocal(localIndex);
        public static ILEmitterFunc LoadNull() => il => il.LoadNull();
        public static ILEmitterFunc LoadString(string value) => il => il.LoadString(value);
        public static ILEmitterFunc MarkLabel(Label label) => il => il.MarkLabel(label);
        public static ILEmitterFunc Ret(int value) => il => il.Return(value);
        public static ILEmitterFunc Ret(LocalBuilder local) => il => il.Return(local);
        public static ILEmitterFunc ShiftLeft(ILEmitterFunc value, ILEmitterFunc numberOfBits) => il => il.ShiftLeft(value, numberOfBits);
    }
}
