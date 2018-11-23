using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Visitor
{
    internal sealed class AutoVisitor
    {
        private readonly ModuleBuilder _moduleBuilder;
        private readonly bool _strict;
        private readonly string _visitMethodName;

        public AutoVisitor() : this(true, "Visit") { }

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
            var typeOfAcceptor = typeof(TAcceptor);
            var typeOfVisitor = typeof(TVisitor);

            var methods = Cache<TAcceptor, TVisitor, TState>
                          .Delegates
                          .GetOrAdd(
                              typeOfAcceptor,
                              _ => new ConcurrentDictionary<Type, Cache<TAcceptor, TVisitor, TState>.VisitDelegate>());

            var visitorMethod = methods.GetOrAdd(
                typeOfVisitor,
                BuildMethod<TAcceptor, TVisitor, TState>);

            if (visitorMethod == null)
            {
                return _strict
                    ? throw new NotSupportedException($"Visitor {typeOfVisitor} can't visit {typeOfAcceptor}.")
                    : state;
            }

            return visitorMethod(visitor, acceptor, state);
        }

        private Cache<TAcceptor, TVisitor, TState>.VisitDelegate
            BuildMethod<TAcceptor, TVisitor, TState>(Type typeOfVisitor) =>
            BuildStaticMethod(typeof(TAcceptor), typeOfVisitor, typeof(TState))
                .CreateDelegate<Cache<TAcceptor, TVisitor, TState>.VisitDelegate>();

        private MethodInfo GetVisitMethod(Type typeOfAcceptor, Type typeOfVisitor, Type typeOfState) =>
            typeOfVisitor
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(methodInfo =>
                {
                    var arguments1 = methodInfo.GetGenericArguments();

                    return methodInfo.Name == _visitMethodName
                           && arguments1.Length == 2
                           && arguments1[0] == typeOfAcceptor
                           && arguments1[1] == typeOfState
                           && methodInfo.ReturnType == typeOfState;
                })
                .SingleOrDefault();

        private MethodInfo BuildStaticMethod(
            Type typeOfAcceptor,
            Type typeOfVisitor,
            Type typeOfState)
        {
            var visitMethod = GetVisitMethod(typeOfAcceptor, typeOfVisitor, typeOfState);
            if (visitMethod == null)
            {
                return null;
            }

            var typeBuilder = DefineStaticClass($"{typeOfAcceptor}_{typeOfVisitor}_Acceptor");
            var parameterTypes = new[] { typeOfVisitor, typeOfAcceptor, typeOfState };
            var methodBuilder = typeBuilder.DefineStaticMethod(
                nameof(Accept),
                typeOfState,
                parameterTypes);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(typeOfVisitor.IsSealed ? OpCodes.Call : OpCodes.Callvirt, visitMethod);
            il.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo();

            return typeInfo.GetMethod(nameof(Accept), parameterTypes);
        }

        private static class Cache<TAcceptor, TVisitor, TState>
        {
            public delegate TState VisitDelegate(TVisitor visitor, TAcceptor acceptor, TState state);

            public static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, VisitDelegate>> Delegates;

            static Cache() => Delegates = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, VisitDelegate>>();
        }
    }
}
