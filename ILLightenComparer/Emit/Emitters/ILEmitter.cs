using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

#if DEBUG
using System.Linq;
#endif

namespace ILLightenComparer.Emit.Emitters
{
    using Locals = Dictionary<byte, Dictionary<Type, LocalBuilder>>;

    internal sealed class ILEmitter : IDisposable
    {
        private const byte ShortFormLimit = byte.MaxValue; // 255

#if DEBUG
        private readonly List<Label> _debugLabels = new List<Label>();
#endif
        private ILGenerator _il;

        private Locals _localBuckets = new Locals
        {
            { 0, new Dictionary<Type, LocalBuilder>() }
        };

        public ILEmitter(ILGenerator il) => _il = il;

        public void Dispose()
        {
#if DEBUG
            var locals = _localBuckets.Values.SelectMany(x => x.Values).ToArray();
            if (locals.Length != 0)
            {
                Debug.WriteLine("\t.locals init (");
                foreach (var item in locals)
                {
                    Debug.WriteLine($"\t\t[{item.LocalIndex}] {item.LocalType}");
                }

                Debug.WriteLine("\t)");
            }
#endif
            _il = null;
            _localBuckets = null;
        }

        public ILEmitter Emit(OpCode opCode)
        {
            Debug.WriteLine($"\t\t{opCode}");

            _il.Emit(opCode);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, int arg)
        {
            Debug.WriteLine($"\t\t{opCode} {arg}");

            _il.Emit(opCode, arg);

            return this;
        }

        public ILEmitter DefineLabel(out Label label)
        {
            label = _il.DefineLabel();
#if DEBUG
            _debugLabels.Add(label);
#endif
            return this;
        }

        public ILEmitter MarkLabel(Label label)
        {
#if DEBUG
            Debug.WriteLine($"\tLabel_{_debugLabels.IndexOf(label)}:");
#endif
            _il.MarkLabel(label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, Label label)
        {
#if DEBUG
            Debug.WriteLine($"\t\t{opCode} Label_{_debugLabels.IndexOf(label)}");
#endif
            _il.Emit(opCode, label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, MethodInfo methodInfo)
        {
            Debug.WriteLine($"\t\t{opCode} {methodInfo.DisplayName()}");

            _il.Emit(opCode, methodInfo);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            Debug.WriteLine($"\t\t{opCode} {field.DisplayName()}");

            _il.Emit(opCode, field);

            return this;
        }

        public ILEmitter Call(Type methodOwner, MethodInfo methodInfo)
        {
            var opCode = methodOwner == null || methodOwner.IsValueType || methodOwner.IsSealed
                ? OpCodes.Call
                : OpCodes.Callvirt;

            return Emit(opCode, methodInfo);
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

        public ILEmitter EmitCtorCall(ConstructorInfo constructor)
        {
            _il.Emit(OpCodes.Newobj, constructor);
            Debug.WriteLine($"\t\t{OpCodes.Newobj} {constructor.DisplayName()}");

            return Emit(OpCodes.Ret);
        }

        public ILEmitter LoadArgument(ushort argumentIndex)
        {
            switch (argumentIndex)
            {
                case 0: return Emit(OpCodes.Ldarg_0);
                case 1: return Emit(OpCodes.Ldarg_1);
                case 2: return Emit(OpCodes.Ldarg_2);
                case 3: return Emit(OpCodes.Ldarg_3);
                default:
                    var opCode = argumentIndex <= ShortFormLimit ? OpCodes.Ldarg_S : OpCodes.Ldarg;
                    return Emit(opCode, argumentIndex);
            }
        }

        public ILEmitter LoadArgumentAddress(ushort argumentIndex)
        {
            var opCode = argumentIndex <= ShortFormLimit ? OpCodes.Ldarga_S : OpCodes.Ldarga;
            return Emit(opCode, argumentIndex);
        }

        public ILEmitter Branch(OpCode opCode, out Label label)
        {
            if (opCode.FlowControl != FlowControl.Branch
                && opCode.FlowControl != FlowControl.Cond_Branch)
            {
                throw new ArgumentOutOfRangeException(nameof(opCode),
                    $"Only a branch instruction is allowed. OpCode: {opCode}.");
            }

            return DefineLabel(out label).Emit(opCode, label);
        }

        public ILEmitter LoadLocal(LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0: return Emit(OpCodes.Ldloc_0);
                case 1: return Emit(OpCodes.Ldloc_1);
                case 2: return Emit(OpCodes.Ldloc_2);
                case 3: return Emit(OpCodes.Ldloc_3);
                default:
                    var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Ldloc_S : OpCodes.Ldloc;
                    return Emit(opCode, local.LocalIndex);
            }
        }

        public ILEmitter LoadAddress(LocalBuilder local)
        {
            var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Ldloca_S : OpCodes.Ldloca;

            Debug.WriteLine($"\t\t{opCode} {local.LocalIndex}");

            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter Store(LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0: return Emit(OpCodes.Stloc_0);
                case 1: return Emit(OpCodes.Stloc_1);
                case 2: return Emit(OpCodes.Stloc_2);
                case 3: return Emit(OpCodes.Stloc_3);

                default:
                    var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Stloc_S : OpCodes.Stloc;
                    Debug.WriteLine($"\t\t{opCode} {local.LocalIndex}");
                    _il.Emit(opCode, local);
                    return this;
            }
        }

        public ILEmitter DeclareLocal(Type localType, out LocalBuilder local) =>
            DeclareLocal(localType, out local, 0);

        public ILEmitter DeclareLocal(Type localType, out LocalBuilder local, byte bucket)
        {
            if (!_localBuckets.TryGetValue(bucket, out var locals))
            {
                locals = _localBuckets[bucket] = new Dictionary<Type, LocalBuilder>();
            }

            if (!locals.TryGetValue(localType, out local))
            {
                local = locals[localType] = _il.DeclareLocal(localType);
            }

            return this;
        }
    }
}
