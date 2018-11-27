using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly CompareEmitter _compareEmitter;
        private readonly TypeBuilderContext _context;
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(TypeBuilderContext context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
            _compareEmitter = new CompareEmitter(_context);
        }

        public TypeInfo Build(Type objectType)
        {
            var basicInterface = typeof(IComparer);
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.Comparer",
                basicInterface,
                genericInterface
            );

            var staticCompare = BuildStaticCompareMethod(typeBuilder, objectType);

            BuildBasicCompareMethod(
                typeBuilder,
                staticCompare,
                basicInterface.GetMethod(MethodName.Compare),
                objectType);

            BuildTypedCompareMethod(
                typeBuilder,
                staticCompare,
                genericInterface.GetMethod(MethodName.Compare));

            return typeBuilder
                   .BuildFactoryMethod()
                   .CreateTypeInfo();
        }

        private MethodBuilder BuildStaticCompareMethod(TypeBuilder typeBuilder, Type objectType)
        {
            var staticMethodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.Compare,
                typeof(int),
                new[]
                {
                    typeof(HashSet<object>),
                    objectType, // x
                    objectType // y
                });

            using (var il = staticMethodBuilder.CreateILEmitter())
            {
                if (objectType.IsClass)
                {
                    EmitReferenceComparision(il);
                }

                EmitMembersComparision(il, objectType);
                EmitDefaultResult(il);
            }

            return staticMethodBuilder;
        }

        private void EmitMembersComparision(ILEmitter il, Type objectType)
        {
            var members = _membersProvider.GetMembers(objectType);

            InitFirstLocalToKeepComparisonsResult(il);
            foreach (var member in members)
            {
                member.Accept(_compareEmitter, il);
            }
        }

        private static void InitFirstLocalToKeepComparisonsResult(ILEmitter il)
        {
            il.DeclareLocal(typeof(int), out _); // todo: automatically create local when needs
        }

        private static void EmitDefaultResult(ILEmitter il)
        {
            il.Emit(OpCodes.Ldc_I4_0)
              .Emit(OpCodes.Ret);
        }

        private static void EmitReferenceComparision(ILEmitter il)
        {
            // x == y
            il.Emit(OpCodes.Ldarg_1)
              .Emit(OpCodes.Ldarg_2)
              .DefineLabel(out var else0)
              .Emit(OpCodes.Bne_Un_S, else0)
              .Emit(OpCodes.Ldc_I4_0) // return 0
              .Emit(OpCodes.Ret)
              .MarkLabel(else0)
              // y != null
              .Emit(OpCodes.Ldarg_2)
              .DefineLabel(out var else1)
              .Emit(OpCodes.Brtrue_S, else1)
              .Emit(OpCodes.Ldc_I4_1) // return 1
              .Emit(OpCodes.Ret)
              .MarkLabel(else1)
              // x != null
              .Emit(OpCodes.Ldarg_1)
              .DefineLabel(out var else2)
              .Emit(OpCodes.Brtrue_S, else2)
              .Emit(OpCodes.Ldc_I4_M1) // return -1
              .Emit(OpCodes.Ret)
              .MarkLabel(else2);
        }

        private static void BuildBasicCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod,
            Type objectType)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                if (objectType.IsValueType)
                {
                    EmitReferenceComparision(il);
                }

                il.Emit(OpCodes.Ldc_I4_0) // todo: hash set to detect cycles
                  .Emit(OpCodes.Ldarg_1)
                  .EmitCast(objectType)
                  .Emit(OpCodes.Ldarg_2)
                  .EmitCast(objectType)
                  .Emit(OpCodes.Call, staticCompareMethod)
                  .Emit(OpCodes.Ret);
            }
        }

        private static void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.Emit(OpCodes.Ldc_I4_0) // todo: hash set to detect cycles
                  .Emit(OpCodes.Ldarg_1)
                  .Emit(OpCodes.Ldarg_2)
                  .Emit(OpCodes.Call, staticCompareMethod)
                  .Emit(OpCodes.Ret);
            }
        }
    }
}
