using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit
{
    internal sealed class ILEmitter
    {
#if DEBUG
        private readonly List<Label> _labels = new List<Label>();
#endif
        private readonly Dictionary<Type, LocalBuilder> _locals = new Dictionary<Type, LocalBuilder>();
        private readonly ILGenerator _il;

        public ILEmitter(ILGenerator il) => _il = il;

        public LocalBuilder GetLocal(Type localType) => _locals.TryGetValue(localType, out var local)
            ? local
            : _locals[localType] = _il.DeclareLocal(localType);

        public Label DefineLabel()
        {
            var label = _il.DefineLabel();
#if DEBUG
            _labels.Add(label);
#endif
            return label;
        }

        public ILEmitter MarkLabel(Label label)
        {
#if DEBUG
            Debug.WriteLine("Label: " + _labels.IndexOf(label));
#endif
            _il.MarkLabel(label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, Label label)
        {
#if DEBUG
            Debug.WriteLine($"{opCode} {_labels.IndexOf(label)}");
#endif
            _il.Emit(opCode, label);

            return this;
        }

        public ILEmitter EmitCast(Type objectType)
        {
            var castOp = objectType.IsValueType
                ? OpCodes.Unbox_Any
                : OpCodes.Castclass;

            Debug.WriteLine($"{castOp} {objectType.Name}");

            _il.Emit(castOp, objectType);

            return this;
        }

        public ILEmitter EmitCtorCall(ConstructorInfo constructor) => EmitNew(constructor).Emit(OpCodes.Ret);

        private ILEmitter EmitNew(ConstructorInfo constructorInfo)
        {
            _il.Emit(OpCodes.Newobj, constructorInfo);

            Debug.WriteLine($"{OpCodes.Newobj} {constructorInfo.Name}");

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
            Debug.WriteLine($"{opCode} {methodInfo.Name}");

            _il.Emit(opCode, methodInfo);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, int arg)
        {
            Debug.WriteLine($"{opCode} {arg}");

            _il.Emit(opCode, arg);

            return this;
        }

        public ILEmitter EmitLoadAddress(LocalBuilder local)
        {
            // todo: test
            var opCode = local.LocalIndex <= 255 ? OpCodes.Ldloca_S : OpCodes.Ldloca;

            Debug.WriteLine($"{opCode} {local.LocalIndex}");

            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter EmitStore(LocalBuilder local)
        {
            Debug.WriteLine($"{OpCodes.Stloc} {local.LocalIndex}");

            _il.Emit(OpCodes.Stloc, local); // todo: use short form

            return this;
        }

        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            Debug.WriteLine($"{opCode} {field.DisplayName()}");

            _il.Emit(opCode, field);

            return this;
        }
    }
}
