/*
   ___ ___ _  _ ___ ___    _ _____ ___ ___     ___ ___  ___  ___
  / __| __| \| | __| _ \  /_\_   _| __|   \   / __/ _ \|   \| __|
 | (_ | _|| .` | _||   / / _ \| | | _|| |) | | (_| (_) | |) | _|
  \___|___|_|\_|___|_|_\/_/ \_\_| |___|___/   \___\___/|___/|___|

*/

using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator.Extensions;

namespace Illuminator
{
    public static class FunctionalExtensions
    {
        public static ILEmitterFunc CallMethod(MethodInfo methodInfo, Type[] parameterTypes, params ILEmitterFunc[] parameters)
            => (in ILEmitter il) => il.CallMethod(methodInfo, parameterTypes, parameters);

        public static ILEmitterFunc CallMethod(MethodInfo methodInfo, Type[] parameterTypes)
            => (in ILEmitter il) => il.CallMethod(methodInfo, parameterTypes);

        public static ILEmitterFunc CallMethod(ConstructorInfo constructor, Type[] parameterTypes)
            => (in ILEmitter il) => il.CallMethod(constructor, parameterTypes);

        public static ILEmitterFunc Cast(Type type) => (in ILEmitter il) => il.Cast(type);
        public static ILEmitterFunc Cast<T>(ILEmitterFunc value) => (in ILEmitter il) => il.Cast<T>(value);

        public static ILEmitterFunc EmitIf(bool condition, params ILEmitterFunc[] actions) => (in ILEmitter il) => il.ExecuteIf(condition, actions);
        public static ILEmitterFunc Greater_S(ILEmitterFunc a, ILEmitterFunc b, Label label) => (in ILEmitter il) => il.Greater_S(a, b, label);
        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue, ILEmitterFunc elseAction) => (in ILEmitter il) => il.If(action, whenTrue, elseAction);
        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue) => (in ILEmitter il) => il.If(action, whenTrue);
        public static ILEmitterFunc If_S(ILEmitterFunc action, ILEmitterFunc whenTrue, ILEmitterFunc elseAction) => (in ILEmitter il) => il.If_S(action, whenTrue, elseAction);
        public static ILEmitterFunc If_S(ILEmitterFunc action, ILEmitterFunc whenTrue) => (in ILEmitter il) => il.If_S(action, whenTrue);

        public static ILEmitterFunc LessOrEqual_S(ILEmitterFunc a, ILEmitterFunc b, Label label) => (in ILEmitter il) => il.LessOrEqual_S(a, b, label);
        public static ILEmitterFunc LoadAddress(LocalVariableInfo local) => (in ILEmitter il) => il.LoadAddress(local);
        public static ILEmitterFunc LoadCaller(LocalVariableInfo local) => (in ILEmitter il) => il.LoadCaller(local);
        public static ILEmitterFunc LoadLocal(LocalVariableInfo local) => (in ILEmitter il) => il.LoadLocal(local);
        public static ILEmitterFunc LoadLocal(int localIndex) => (in ILEmitter il) => il.LoadLocal(localIndex);
        public static ILEmitterFunc LoadNull() => (in ILEmitter il) => il.LoadNull();
        public static ILEmitterFunc LoadString(string value) => (in ILEmitter il) => il.LoadString(value);
        public static ILEmitterFunc MarkLabel(Label label) => (in ILEmitter il) => il.MarkLabel(label);
        public static ILEmitterFunc Ret(int value) => (in ILEmitter il) => il.Return(value);
        public static ILEmitterFunc Ret(LocalBuilder local) => (in ILEmitter il) => il.Return(local);
        public static ILEmitterFunc ShiftLeft(ILEmitterFunc value, ILEmitterFunc numberOfBits) => (in ILEmitter il) => il.ShiftLeft(value, numberOfBits);
    }
}