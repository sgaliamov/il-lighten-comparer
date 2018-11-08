using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Methods;
using ILLightenComparer.Emit.Types;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerEmitter
    {
        private readonly TypeEmitter _emitter;
        private readonly CompareMethodEmitter _methodEmitter = new CompareMethodEmitter();

        public ComparerEmitter(TypeEmitter emitter) => _emitter = emitter;

        public IComparer Emit(Type objectType, CompareConfiguration configuration)
        {
            var type = _emitter.DefineType($"{objectType.FullName}.Comparer");

            var method = _emitter.DefineInterfaceMethod(type, Method.Compare);

            _methodEmitter.Emit(objectType, configuration, method);

            return Create<IComparer>(type);
        }

        public IComparer<T> Emit<T>(CompareConfiguration configuration)
        {
            var objectType = typeof(T);

            var type = _emitter.DefineType($"{objectType.FullName}.Comparer");

            var method = _emitter.DefineInterfaceMethod(type, Method.GenericCompare);

            _methodEmitter.Emit(objectType, configuration, method);

            return Create<IComparer<T>>(type);
        }

        private T Create<T>(TypeBuilder type) => _emitter.EmitFactoryMethod<T>(type)();
    }
}
