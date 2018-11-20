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

        public Label DefineLabel() => _il.DefineLabel();

        public ILEmitter MarkLabel(Label label)
        {
            _il.MarkLabel(label);

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

        public ILEmitter EmitCallCtor(ConstructorInfo constructor)
        {
            _il.Emit(OpCodes.Newobj, constructor);
            _il.Emit(OpCodes.Ret);

            return this;
        }

        public ILEmitter Emit(OpCode opCode)
        {
            Debug.WriteLine(opCode.ToString());
            _il.Emit(opCode);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, MethodInfo methodInfo)
        {
            _il.Emit(opCode, methodInfo);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, int arg)
        {
            _il.Emit(opCode, arg);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, Label label)
        {
            _il.Emit(opCode, label);

            return this;
        }

        public ILEmitter EmitLoadAddress(LocalBuilder local)
        {
            var opCode = local.LocalIndex <= 255 ? OpCodes.Ldloca_S : OpCodes.Ldloca;

            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter EmitStore(LocalBuilder local)
        {
            _il.Emit(OpCodes.Stloc, local); // todo: use short form

            return this;
        }
    }
}
