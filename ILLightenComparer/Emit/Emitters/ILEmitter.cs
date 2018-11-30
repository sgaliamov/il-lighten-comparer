using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    using Locals = Dictionary<byte, Dictionary<Type, LocalBuilder>>;

    // ReSharper disable once PartialTypeWithSinglePart
    internal sealed partial class ILEmitter : IDisposable
    {
        private const byte ShortFormLimit = byte.MaxValue; // 255

        private ILGenerator _il;

        private Locals _localBuckets = new Locals
        {
            { 0, new Dictionary<Type, LocalBuilder>() }
        };

        public ILEmitter(ILGenerator il) => _il = il;

        public void Dispose()
        {
            DebugOutput();
            _il = null;
            _localBuckets = null;
        }

        public ILEmitter Emit(OpCode opCode)
        {
            DebugLine($"\t\t{opCode}");
            _il.Emit(opCode);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, int arg)
        {
            DebugLine($"\t\t{opCode} {arg}");
            _il.Emit(opCode, arg);

            return this;
        }

        public ILEmitter DefineLabel(out Label label)
        {
            label = _il.DefineLabel();
            AddDebugLabel(label);

            return this;
        }

        public ILEmitter MarkLabel(Label label)
        {
            DebugMarkLabel(label);
            _il.MarkLabel(label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, Label label)
        {
            DebugEmitLabel(opCode, label);
            _il.Emit(opCode, label);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, MethodInfo methodInfo)
        {
            DebugLine($"\t\t{opCode} {methodInfo.DisplayName()}");
            _il.Emit(opCode, methodInfo);

            return this;
        }

        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            DebugLine($"\t\t{opCode} {field.DisplayName()}");
            _il.Emit(opCode, field);

            return this;
        }

        public ILEmitter Call(MethodInfo methodInfo)
        {
            var owner = methodInfo.DeclaringType;
            if (owner == null)
            {
                throw new InvalidOperationException(
                    $"It's not expected that {methodInfo.DisplayName()} doesn't have a declaring type.");
            }

            var opCode = methodInfo.IsStatic || owner.IsValueType || owner.IsSealed
                ? OpCodes.Call
                : OpCodes.Callvirt;

            return Emit(opCode, methodInfo);
        }

        public ILEmitter Return(int value) => LoadConstant(value).Emit(OpCodes.Ret);

        public ILEmitter EmitCast(Type objectType)
        {
            var castOp = objectType.IsValueType
                ? OpCodes.Unbox_Any
                : OpCodes.Castclass;

            DebugLine($"\t\t{castOp} {objectType.Name}");
            _il.Emit(castOp, objectType);

            return this;
        }

        public ILEmitter EmitCtorCall(ConstructorInfo constructor)
        {
            DebugLine($"\t\t{OpCodes.Newobj} {constructor.DisplayName()}");
            _il.Emit(OpCodes.Newobj, constructor);

            return Emit(OpCodes.Ret);
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

        public ILEmitter LoadConstant(int value)
        {
            switch (value)
            {
                case -1: return Emit(OpCodes.Ldc_I4_M1);
                case 0: return Emit(OpCodes.Ldc_I4_0);
                case 1: return Emit(OpCodes.Ldc_I4_1);
                case 2: return Emit(OpCodes.Ldc_I4_2);
                case 3: return Emit(OpCodes.Ldc_I4_3);
                case 4: return Emit(OpCodes.Ldc_I4_4);
                case 5: return Emit(OpCodes.Ldc_I4_5);
                case 6: return Emit(OpCodes.Ldc_I4_6);
                case 7: return Emit(OpCodes.Ldc_I4_7);
                case 8: return Emit(OpCodes.Ldc_I4_8);
                default:
                    var opCode = value <= ShortFormLimit ? OpCodes.Ldc_I4_S : OpCodes.Ldc_I4;
                    return Emit(opCode, value);
            }
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
            DebugLine($"\t\t{opCode} {local.LocalIndex}");
            _il.Emit(opCode, local);

            return this;
        }

        public ILEmitter Store(Type localType, out LocalBuilder local) => Store(localType, 0, out local);

        public ILEmitter Store(Type localType, byte bucket, out LocalBuilder local)
        {
            DeclareLocal(localType, bucket, out local);

            switch (local.LocalIndex)
            {
                case 0: return Emit(OpCodes.Stloc_0);
                case 1: return Emit(OpCodes.Stloc_1);
                case 2: return Emit(OpCodes.Stloc_2);
                case 3: return Emit(OpCodes.Stloc_3);

                default:
                    var opCode = local.LocalIndex <= ShortFormLimit ? OpCodes.Stloc_S : OpCodes.Stloc;
                    DebugLine($"\t\t{opCode} {local.LocalIndex}");
                    _il.Emit(opCode, local);
                    return this;
            }
        }

        public void DeclareLocal(Type localType, byte bucket, out LocalBuilder local)
        {
            if (!_localBuckets.TryGetValue(bucket, out var locals))
            {
                locals = _localBuckets[bucket] = new Dictionary<Type, LocalBuilder>();
            }

            if (!locals.TryGetValue(localType, out local))
            {
                local = locals[localType] = _il.DeclareLocal(localType);
            }
        }

        #region debug

        // ReSharper disable PartialMethodWithSinglePart

        partial void DebugOutput();
        partial void DebugEmitLabel(OpCode opCode, Label label);
        partial void DebugMarkLabel(Label label);
        partial void DebugLine(string message);
        partial void AddDebugLabel(Label label);

        // ReSharper restore PartialMethodWithSinglePart

        #endregion
    }
}
