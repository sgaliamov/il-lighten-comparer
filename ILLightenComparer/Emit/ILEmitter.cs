using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit
{
    internal sealed class ILEmitter
    {
        private readonly Dictionary<Type, LocalBuilder> _cache = new Dictionary<Type, LocalBuilder>(0);
        private readonly ILGenerator _il;

        public ILEmitter(ILGenerator il) => _il = il;

        public LocalBuilder GetLocal(Type localType) =>
            _cache.TryGetValue(localType, out var local)
                ? local
                : _cache[localType] = _il.DeclareLocal(localType);

        public ILEmitter Emit(OpCode opCode)
        {
            Debug.WriteLine(opCode.ToString());
            _il.Emit(opCode);

            return this;
        }

        public ILEmitter EmitCast(Type objectType)
        {
            var castOp = objectType.IsValueType
                ? OpCodes.Unbox_Any
                : OpCodes.Castclass;

            _il.Emit(castOp, objectType);

            return this;
        }

        public ILEmitter EmitCall(MethodInfo methodInfo)
        {
            _il.Emit(OpCodes.Call, methodInfo);

            return this;
        }

        public ILEmitter EmitCallCtor(ConstructorInfo constructor)
        {
            _il.Emit(OpCodes.Newobj, constructor);
            _il.Emit(OpCodes.Ret);

            return this;
        }

        public Label DefineLabel() => _il.DefineLabel();

        public ILEmitter MarkLabel(Label label)
        {
            _il.MarkLabel(label);

            return this;
        }

        internal ILEmitter Emit(OpCode opCode, Label label)
        {
            _il.Emit(opCode, label);

            return this;
        }
    }
}
