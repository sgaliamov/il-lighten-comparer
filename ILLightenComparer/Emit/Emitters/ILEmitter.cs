using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class ILEmitter : IDisposable
    {
        private const byte ShortFormLimit = byte.MaxValue; // 255
#if DEBUG
        private readonly List<Label> _labels = new List<Label>();
#endif
        private ILGenerator _il;
        private Dictionary<Type, LocalBuilder> _locals = new Dictionary<Type, LocalBuilder>();

        public ILEmitter(ILGenerator il) => _il = il;

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

        public LocalBuilder DeclareLocal(Type localType) =>
            _locals.TryGetValue(localType, out var local)
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

        public ILEmitter LoadArgument(ushort index)
        {
            switch (index)
            {
                case 0:
                    Emit(OpCodes.Ldarg_0);
                    return this;

                case 1:
                    Emit(OpCodes.Ldarg_1);
                    return this;

                case 2:
                    Emit(OpCodes.Ldarg_2);
                    return this;

                case 3:
                    Emit(OpCodes.Ldarg_3);
                    return this;

                default:
                    var opCode = index <= ShortFormLimit ? OpCodes.Ldarg_S : OpCodes.Ldarg;
                    Debug.WriteLine($"\t\t{opCode} {index}");
                    Emit(opCode, index);
                    return this;
            }
        }

        public ILEmitter LoadAddress(ushort argumentIndex)
        {
            var opCode = argumentIndex <= ShortFormLimit ? OpCodes.Ldarga_S : OpCodes.Ldarga;
            return Emit(opCode, argumentIndex);
        }

        public ILEmitter EmitLoadAddressOf(LocalBuilder local)
        {
            var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Ldloca_S : OpCodes.Ldloca;

            Debug.WriteLine($"\t\t{opCode} {local.LocalIndex}");

            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter EmitStore(LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0:
                    Emit(OpCodes.Stloc_0);
                    return this;

                case 1:
                    Emit(OpCodes.Stloc_1);
                    return this;

                case 2:
                    Emit(OpCodes.Stloc_2);
                    return this;

                case 3:
                    Emit(OpCodes.Stloc_3);
                    return this;

                default:
                    var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Stloc_S : OpCodes.Stloc;
                    Debug.WriteLine($"\t\t{OpCodes.Stloc} {local.LocalIndex}");
                    _il.Emit(opCode, local);
                    return this;
            }
        }

        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            Debug.WriteLine($"\t\t{opCode} {field.DisplayName()}");

            _il.Emit(opCode, field);

            return this;
        }

        private ILEmitter EmitNew(ConstructorInfo constructorInfo)
        {
            _il.Emit(OpCodes.Newobj, constructorInfo);

            Debug.WriteLine($"\t\t{OpCodes.Newobj} {constructorInfo.DisplayName()}");

            return this;
        }
    }
}
