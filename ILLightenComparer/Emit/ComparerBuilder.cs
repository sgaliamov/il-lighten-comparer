using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Methods;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerBuilder
    {
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();
        private readonly Context _context;
        private readonly CompareMethodEmitter _methodEmitter;

        public ComparerBuilder(Context context)
        {
            _context = context;
            _methodEmitter = new CompareMethodEmitter(_context);
        }

        public IComparer Build(Type objectType) => _comparers.GetOrAdd(objectType, Create);

        public IComparer<T> Build<T>() => (IComparer<T>)_comparers.GetOrAdd(typeof(T), Create);

        private IComparer Create(Type objectType)
        {
            var typeBuilder = _context.DefineType($"{objectType.FullName}.Comparer", typeof(IComparer));

            var methodBuilder = _context.DefineInterfaceMethod(typeBuilder, Method.Compare);

            EmitCall(
                _methodEmitter.Emit(objectType),
                methodBuilder.GetILGenerator());

            return Create<IComparer>(typeBuilder);
        }

        private static void EmitCall(MethodInfo methodInfo, ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);
        }

        private T Create<T>(TypeBuilder typeBuilder)
        {
            var typeInfo = typeBuilder.CreateTypeInfo();

            return _context.EmitFactoryMethod<T>(typeInfo)();
        }
    }
}
