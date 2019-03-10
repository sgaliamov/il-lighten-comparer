using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emitters.Builders
{
    // todo: one cache for types and clean on configuration change
    internal sealed class ContextBuilder : IContextBuilder
    {
        private readonly ConcurrentDictionary<Type, Lazy<ComparerInfo>> _infos = new ConcurrentDictionary<Type, Lazy<ComparerInfo>>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;

        public ContextBuilder(IConfigurationProvider configurations)
        {
            _comparerTypeBuilder = new ComparerTypeBuilder(this, configurations);

            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.dll");
        }

        public MethodInfo GetStaticCompareMethod(Type type)
        {
            // todo: test when a custom comparer is defined for nested type
            return DefineComparerInfo(type).CompareMethod;
        }

        public MethodInfo GetCompiledCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public Type GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                key => new Lazy<Type>(() =>
                {
                    var info = DefineComparerInfo(key);

                    var compiledComparerType = _comparerTypeBuilder.Build(
                        (TypeBuilder)info.CompareMethod.DeclaringType,
                        (MethodBuilder)info.CompareMethod,
                        key);

                    var method = compiledComparerType.GetMethod(MethodName.Compare, Method.StaticCompareMethodParameters(key));

                    info.FinalizeBuild(method);

                    return compiledComparerType;
                }));

            var comparerType = lazy.Value;

            FinalizeStartedBuilds();

            return comparerType;
        }

        private ComparerInfo DefineComparerInfo(Type objectType)
        {
            var lazy = _infos.GetOrAdd(objectType,
                key => new Lazy<ComparerInfo>(() =>
                {
                    var genericInterface = typeof(IComparer<>).MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{key.FullName}.DynamicComparer",
                        genericInterface);

                    var staticCompareMethodBuilder = typeBuilder.DefineStaticMethod(
                        MethodName.Compare,
                        typeof(int),
                        Method.StaticCompareMethodParameters(key));

                    return new ComparerInfo(staticCompareMethodBuilder);
                }));

            return lazy.Value;
        }

        private void FinalizeStartedBuilds()
        {
            var items = _infos.ToDictionary(x => x.Key, x => x.Value.Value);

            foreach (var item in items)
            {
                if (item.Value.Compiled)
                {
                    continue;
                }

                GetComparerType(item.Key);
            }
        }
    }

    internal interface IContextBuilder
    {
        MethodInfo GetStaticCompareMethod(Type type);
    }
}
