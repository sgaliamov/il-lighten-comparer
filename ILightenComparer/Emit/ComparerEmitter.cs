using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILightenComparer.Emit.Methods;
using ILightenComparer.Emit.Types;
using ILightenComparer.Reflection;

namespace ILightenComparer.Emit
{
    internal sealed class ComparerEmitter
    {
        private readonly TypeEmitter _emitter;
        private readonly CompareMethodEmitter _methodEmitter = new CompareMethodEmitter();

        public ComparerEmitter(TypeEmitter emitter) => _emitter = emitter;

        public IComparer Emit(Type objectType, CompareConfiguration configuration)
        {
            var type = EmitType(objectType, configuration);

            return Create<IComparer>(type);
        }

        public IComparer<T> Emit<T>(CompareConfiguration configuration)
        {
            var type = EmitType(typeof(T), configuration);

            return Create<IComparer<T>>(type);
        }

        private T Create<T>(TypeBuilder type) => _emitter.EmitFactoryMethod<T>(type)();

        private TypeBuilder EmitType(Type objectType, CompareConfiguration configuration)
        {
            var type = _emitter.DefineType($"{objectType.FullName}.Comparer");

            var method = _emitter.DefineInterfaceMethod(type, Method.Compare);

            _methodEmitter.Emit(objectType, configuration, method);

            return type;
        }
    }
}
