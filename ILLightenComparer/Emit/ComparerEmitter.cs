using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Methods;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerEmitter
    {
        private readonly Context _context;
        private readonly CompareMethodEmitter _methodEmitter;

        public ComparerEmitter(Context context)
        {
            _context = context;
            _methodEmitter = new CompareMethodEmitter(_context);
        }

        public IComparer Emit(Type objectType)
        {
            var type = _context.DefineType($"{objectType.FullName}.Comparer");

            var method = _context.DefineInterfaceMethod(type, Method.Compare);

            _methodEmitter.Emit(objectType, method.GetILGenerator());

            return Create<IComparer>(type);
        }

        public IComparer<T> Emit<T>() => throw new NotImplementedException();

        private T Create<T>(TypeBuilder typeBuilder)
        {
            var typeInfo = typeBuilder.CreateTypeInfo();

            return _context.EmitFactoryMethod<T>(typeInfo)();
        }
    }
}
