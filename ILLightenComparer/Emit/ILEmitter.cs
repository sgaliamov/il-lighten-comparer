using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit
{
    internal sealed class ILEmitter : IDisposable
    {
#if DEBUG
        private readonly List<Label> _labels = new List<Label>();
#endif
        private Dictionary<Type, LocalBuilder> _locals = new Dictionary<Type, LocalBuilder>();
        private ILGenerator _il;

        public ILEmitter(ILGenerator il) => _il = il;

        public LocalBuilder DeclareLocal(Type localType) => _locals.TryGetValue(localType, out var local)
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
            Debug.WriteLine($"\tLabel_{_labels.IndexOf(label)}:");
#endif
            _il.MarkLabel(label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, Label label)
        {
#if DEBUG
            Debug.WriteLine($"\t\t{opCode} Label_{_labels.IndexOf(label)}");
#endif
            _il.Emit(opCode, label);

            return this;
        }

        public ILEmitter EmitCast(Type objectType)
        {
            var castOp = objectType.IsValueType
                ? OpCodes.Unbox_Any
                : OpCodes.Castclass;

            Debug.WriteLine($"\t\t{castOp} {objectType.Name}");

            _il.Emit(castOp, objectType);

            return this;
        }

        public ILEmitter EmitCtorCall(ConstructorInfo constructor) => EmitNew(constructor).Emit(OpCodes.Ret);

        private ILEmitter EmitNew(ConstructorInfo constructorInfo)
        {
            _il.Emit(OpCodes.Newobj, constructorInfo);

            Debug.WriteLine($"\t\t{OpCodes.Newobj} {constructorInfo.DisplayName()}");

            return this;
        }

        public ILEmitter Emit(OpCode opCode)
        {
            Debug.WriteLine($"\t\t{opCode}");

            _il.Emit(opCode);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, MethodInfo methodInfo)
        {
            Debug.WriteLine($"\t\t{opCode} {methodInfo.DisplayName()}");

            _il.Emit(opCode, methodInfo);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, int arg)
        {
            Debug.WriteLine($"\t\t{opCode} {arg}");

            _il.Emit(opCode, arg);

            return this;
        }

        public ILEmitter EmitLoadAddress(LocalBuilder local)
        {
            // todo: test
            var opCode = local.LocalIndex <= 255 ? OpCodes.Ldloca_S : OpCodes.Ldloca;

            Debug.WriteLine($"\t\t{opCode} {local.LocalIndex}");

            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter EmitStore(LocalBuilder local)
        {
            Debug.WriteLine($"\t\t{OpCodes.Stloc} {local.LocalIndex}");

            _il.Emit(OpCodes.Stloc, local); // todo: use short form

            return this;
        }

        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            Debug.WriteLine($"\t\t{opCode} {field.DisplayName()}");

            _il.Emit(opCode, field);

            return this;
        }

        public void Dispose()
        {
#if DEBUG
            if (_locals.Count != 0)
            {
                Debug.WriteLine("\t.locals init (");
                foreach (var item in _locals)
                {
                    Debug.WriteLine($"\t\t[{item.Value.LocalIndex}] {item.Key.Name}");
                }
                Debug.WriteLine("\t)\n");
            }
#endif
            _il = null;
            _locals.Clear();
            _locals = null;
        }
    }
}
