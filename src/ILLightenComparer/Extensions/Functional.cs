/*
   ___ ___ _  _ ___ ___    _ _____ ___ ___     ___ ___  ___  ___
  / __| __| \| | __| _ \  /_\_   _| __|   \   / __/ _ \|   \| __|
 | (_ | _|| .` | _||   / / _ \| | | _|| |) | | (_| (_) | |) | _|
  \___|___|_|\_|___|_|_\/_/ \_\_| |___|___/   \___\___/|___/|___|

*/

using System;
using System.Reflection;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    public static class Functional
    {
        public static ILEmitterFunc CallMethod(MethodInfo methodInfo, Type[] parameterTypes, params ILEmitterFunc[] parameters) =>
            (in ILEmitter il) => il.CallMethod(methodInfo, parameterTypes, parameters);

        public static ILEmitterFunc CallMethod(MethodInfo methodInfo, Type[] parameterTypes) =>
            (in ILEmitter il) => il.CallMethod(methodInfo, parameterTypes);

        public static ILEmitterFunc Cast<T>(ILEmitterFunc value) =>
            (in ILEmitter il) => il.Cast<T>(value);

        public static ILEmitterFunc EmitIf(bool condition, params ILEmitterFunc[] actions) =>
            (in ILEmitter il) => il.ExecuteIf(condition, actions);

        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue, ILEmitterFunc elseAction) =>
            (in ILEmitter il) => il.If(action, whenTrue, elseAction);

        public static ILEmitterFunc If(ILEmitterFunc action, ILEmitterFunc whenTrue) =>
            (in ILEmitter il) => il.If(action, whenTrue);

        public static ILEmitterFunc LoadCaller(LocalBuilder local) =>
            (in ILEmitter il) => il.LoadCaller(local);

        public static ILEmitterFunc Ret(int value) => (in ILEmitter il) => il.Ret(value);

        public static ILEmitterFunc Ret(LocalBuilder local) => (in ILEmitter il) => il.Ret(local);
    }
}