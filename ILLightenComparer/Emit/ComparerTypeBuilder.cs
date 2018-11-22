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
        private readonly TypeBuilderContext _context;
        private readonly MembersProvider _membersProvider;
        private readonly CompareEmitVisitor _visitor;

        public ComparerTypeBuilder(TypeBuilderContext context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
            _visitor = new CompareEmitVisitor(_context);
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
                basicInterface.GetMethod(Constants.CompareMethodName),
                objectType);

            BuildTypedCompareMethod(
                typeBuilder,
                staticCompare,
                genericInterface.GetMethod(Constants.CompareMethodName));

            return typeBuilder
                   .BuildFactoryMethod()
                   .CreateTypeInfo();
        }

        private MethodBuilder BuildStaticCompareMethod(TypeBuilder typeBuilder, Type objectType)
        {
            var staticMethodBuilder = typeBuilder.DefineStaticMethod(
                Constants.CompareMethodName,
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
            var members = _membersProvider.GetMembers(objectType, _context.Configuration);

            InitFirstLocalToKeepComparisonsResult(il);
            foreach (var member in members)
            {
                member.Accept(_visitor, il);
            }
        }

        private static void InitFirstLocalToKeepComparisonsResult(ILEmitter il)
        {
            il.DeclareLocal(typeof(int)); // todo: automatically create local when needs
        }

        private static void EmitDefaultResult(ILEmitter il)
        {
            il.Emit(OpCodes.Ldc_I4_0)
              .Emit(OpCodes.Ret);
        }

        private static void EmitReferenceComparision(ILEmitter il)
        {
            // x == y
            var else0 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Bne_Un_S, else0);
            il.Emit(OpCodes.Ldc_I4_0); // return 0
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else0);

            // y != null
            var else1 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue_S, else1);
            il.Emit(OpCodes.Ldc_I4_1); // return 1
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else1);

            // x != null
            var else2 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, else2);
            il.Emit(OpCodes.Ldc_I4_M1); // return -1
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else2);
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
