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
        private readonly EmitterContext _emitterContext;
        private readonly CompareMethodEmitter _methodEmitter = new CompareMethodEmitter();

        public ComparerEmitter(EmitterContext emitterContext) => _emitterContext = emitterContext;

        public IComparer Emit(Type objectType, CompareConfiguration configuration)
        {
            var type = _emitterContext.DefineType($"{objectType.FullName}.Comparer");

            var method = _emitterContext.DefineInterfaceMethod(type, Method.Compare);

            _methodEmitter.Emit(objectType, configuration, method);

            return Create<IComparer>(type);
        }

        public IComparer<T> Emit<T>(CompareConfiguration configuration) => throw new NotImplementedException();

        private T Create<T>(TypeBuilder typeBuilder)
        {
            var typeInfo = typeBuilder.CreateTypeInfo();

            //var instance = Activator.CreateInstance(typeInfo);

            //return (T)instance;

            return _emitterContext.EmitFactoryMethod<T>(typeInfo)();
        }
    }
}
