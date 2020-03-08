using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ILLightenComparer.Config;
using ILLightenComparer.Reflection;
using Illuminator.Extensions;

[assembly: InternalsVisibleTo("IL-Lighten-Comparer")]

namespace ILLightenComparer.Comparer.Builders
{
    internal sealed class ComparerProvider
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<StaticMethodInfo>> _staticMethods = new ConcurrentDictionary<Type, Lazy<StaticMethodInfo>>();
        private readonly IConfigurationProvider _configurations;

        public ComparerProvider(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _comparerTypeBuilder = new ComparerTypeBuilder(this, _configurations);
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName("IL-Lighten-Comparer"), AssemblyBuilderAccess.RunAndCollect)
                .DefineDynamicModule("IL-Lighten-Comparer.module");
        }

        // method info is enough to emit compare on sealed type
        public MethodInfo GetStaticCompareMethodInfo(Type type) => DefineStaticMethod(type).CompareMethod;

        // is used for delayed calls
        public MethodInfo GetCompiledStaticCompareMethod(Type type)
        {
            EnsureComparerType(type);

            return _staticMethods.TryGetValue(type, out var value) && value.Value.Compiled
                       ? value.Value.CompareMethod
                       : throw new InvalidOperationException("Compiled method is expected.");
        }

        public Type EnsureComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                key => new Lazy<Type>(() => {
                    var info = DefineStaticMethod(key);
                    if (info.Compiled) {
                        throw new InvalidOperationException("Not compiled method is expected.");
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
                key => new Lazy<StaticMethodInfo>(() => {
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

            foreach (var item in items) {
                if (item.Value.Compiled) {
                    continue;
                }

                EnsureComparerType(item.Key);
            }
        }

        private sealed class StaticMethodInfo
        {
            public StaticMethodInfo(MethodInfo compareMethod) => CompareMethod = compareMethod;

            public MethodInfo CompareMethod { get; private set; }

            public bool Compiled { get; private set; }

            public void SetCompiledMethod(MethodInfo method)
            {
                CompareMethod = method;
                Compiled = true;
            }
        }
    }
}
