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
    // todo: clean on configuration change
    internal sealed class ContextBuilder : IContextBuilder
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<StaticMethodInfo>> _staticMethods = new ConcurrentDictionary<Type, Lazy<StaticMethodInfo>>();

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
            return DefineStaticMethod(type).CompareMethod;
        }

        // todo: try replace with GetStaticCompareMethod
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
                    var info = DefineStaticMethod(key);
                    if (info.Compiled)
                    {
                        throw new InvalidOperationException("Unexpected context state.");
                    }

                    var compiledComparerType = _comparerTypeBuilder.Build(
                        (TypeBuilder)info.CompareMethod.DeclaringType,
                        (MethodBuilder)info.CompareMethod,
                        key);

                    var method = compiledComparerType.GetMethod(MethodName.Compare, Method.StaticCompareMethodParameters(key));

                    info.SetCompiledMethod(method);

                    return compiledComparerType;
                }));

            var comparerType = lazy.Value;

            FinalizeStartedBuilds();

            return comparerType;
        }

        private StaticMethodInfo DefineStaticMethod(Type objectType)
        {
            var lazy = _staticMethods.GetOrAdd(objectType,
                key => new Lazy<StaticMethodInfo>(() =>
                {
                    var genericInterface = typeof(IComparer<>).MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{key.FullName}.DynamicComparer",
                        genericInterface);

                    var staticCompareMethodBuilder = typeBuilder.DefineStaticMethod(
                        MethodName.Compare,
                        typeof(int),
                        Method.StaticCompareMethodParameters(key));

                    return new StaticMethodInfo(staticCompareMethodBuilder);
                }));

            return lazy.Value;
        }

        private void FinalizeStartedBuilds()
        {
            var items = _staticMethods.ToDictionary(x => x.Key, x => x.Value.Value);

            foreach (var item in items)
            {
                if (item.Value.Compiled)
                {
                    continue;
                }

                GetComparerType(item.Key);
            }
        }

        private sealed class StaticMethodInfo
        {
            public StaticMethodInfo(MethodInfo compareMethod)
            {
                CompareMethod = compareMethod;
            }

            public MethodInfo CompareMethod { get; private set; }

            public bool Compiled { get; private set; }

            public void SetCompiledMethod(MethodInfo method)
            {
                CompareMethod = method;
                Compiled = true;
            }
        }
    }

    internal interface IContextBuilder
    {
        MethodInfo GetStaticCompareMethod(Type type);
    }
}
