using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Visitor
{
    using DelegatesCollection = ConcurrentDictionary<Type, ConcurrentDictionary<Type, Delegate>>;

    public sealed class AutoVisitor
    {
        private readonly DelegatesCollection _delegates = new DelegatesCollection();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly bool _strict;
        private readonly string _visitMethodName;

        public AutoVisitor() : this(false, "Visit") { }

        public AutoVisitor(bool strict, string visitMethodName)
        {
            _strict = strict;
            _visitMethodName = visitMethodName;

            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer.AutoVisitor"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.AutoVisitor.dll");
        }

        public TypeBuilder DefineStaticClass(string name) =>
            _moduleBuilder.DefineType(
                name,
                TypeAttributes.Sealed
                | TypeAttributes.NotPublic
                | TypeAttributes.Abstract
                | TypeAttributes.Class);

        public TState Accept<TAcceptor, TVisitor, TState>(
            TAcceptor acceptor,
            TVisitor visitor,
            TState state)
        {
            var typeOfAcceptor = acceptor.GetType();
            var typeOfVisitor = visitor.GetType();
            var typeOfState = state.GetType();

            var methods = _delegates
                .GetOrAdd(
                    typeof(TAcceptor),
                    _ => new ConcurrentDictionary<Type, Delegate>());

            var visitorMethod = (Func<TVisitor, TAcceptor, TState, TState>)methods.GetOrAdd(
                typeof(TVisitor),
                _ => BuildMethod<TVisitor, TAcceptor, TState>(typeOfVisitor, typeOfAcceptor, typeOfState));

            if (visitorMethod == null)
            {
                return _strict
                    ? throw new NotSupportedException($"Visitor {typeOfVisitor} can't visit {typeOfAcceptor}.")
                    : state;
            }

            return visitorMethod(visitor, acceptor, state);
        }

        private Func<TVisitor, TAcceptor, TState, TState>
            BuildMethod<TVisitor, TAcceptor, TState>(Type typeOfVisitor, Type typeOfAcceptor, Type typeOfState)
        {
            var visitMethod = GetVisitorMethod(typeOfVisitor, typeOfAcceptor, typeOfState);
            if (visitMethod == null)
            {
                return null;
            }

            var method = BuildStaticMethod<TVisitor, TAcceptor, TState>(
                $"{typeof(TAcceptor)}_{typeof(TVisitor)}_Acceptor",
                typeOfAcceptor,
                typeOfState,
                visitMethod);

            return method.CreateDelegate<Func<TVisitor, TAcceptor, TState, TState>>();
        }

        private MethodInfo GetVisitorMethod(Type typeOfVisitor, Type typeOfAcceptor, Type typeOfState) =>
            typeOfVisitor
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(methodInfo =>
                {
                    var parameters = methodInfo.GetParameters();

                    return methodInfo.Name == _visitMethodName
                           && parameters.Length == 2
                           && parameters[0].ParameterType == typeOfAcceptor
                           && parameters[1].ParameterType == typeOfState
                           && methodInfo.ReturnType == typeOfState;
                })
                .SingleOrDefault();

        private MethodInfo BuildStaticMethod<TVisitor, TAcceptor, TState>(
            string name,
            Type typeOfAcceptor,
            Type typeOfState,
            MethodInfo visitMethod)
        {
            var callTypeOfAcceptor = typeof(TAcceptor);
            var callTypeOfVisitor = typeof(TVisitor);
            var callTypeOfState = typeof(TState);

            var typeBuilder = DefineStaticClass(name);
            var parameterTypes = new[] { callTypeOfVisitor, callTypeOfAcceptor, callTypeOfState };
            var methodBuilder = typeBuilder.DefineStaticMethod(
                nameof(Accept),
                callTypeOfState,
                parameterTypes);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            UnboxCastValue(il, typeOfAcceptor, callTypeOfAcceptor);
            il.Emit(OpCodes.Ldarg_2);
            UnboxCastValue(il, typeOfState, callTypeOfState);
            il.Emit(callTypeOfVisitor.IsSealed ? OpCodes.Call : OpCodes.Callvirt, visitMethod);
            BoxCastValue(il, typeOfState, callTypeOfState);
            il.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo();

            return typeInfo.GetMethod(nameof(Accept), parameterTypes);
        }

        private static void UnboxCastValue(ILGenerator il, Type actualType, Type callType)
        {
            if (callType.IsValueType || !actualType.IsValueType)
            {
                return;
            }

            il.Emit(OpCodes.Unbox_Any, actualType);
        }

        private static void BoxCastValue(ILGenerator il, Type actualType, Type callType)
        {
            if (callType.IsValueType || !actualType.IsValueType)
            {
                return;
            }

            il.Emit(OpCodes.Box, actualType);
        }
    }
}
