using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Illuminator.Extensions;

namespace ILLightenComparer.Shared
{
    internal sealed partial class GenericProvider<TGenericInterface>
    {
        private static readonly MethodInfo[] _interfaceMethods = typeof(TGenericInterface).GetMethods();
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<StaticMethodsInfo>> _methods = new ConcurrentDictionary<Type, Lazy<StaticMethodsInfo>>();
        private readonly Func<StaticMethodsInfo, Type> _buildType;

        public GenericProvider(Func<StaticMethodsInfo, Type> buildType)
        {
            _buildType = buildType;
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName("IL-Lighten-Comparer"), AssemblyBuilderAccess.RunAndCollect)
                .DefineDynamicModule("IL-Lighten-Comparer.module");
        }

        // method info is enough to emit compare on sealed type in IndirectComparison
        public MethodInfo GetStaticMethodInfo(Type type, string name) => DefineStaticMethod(type).GetMethodInfo(name);

        // is used for delayed calls in context
        public MethodInfo GetCompiledStaticMethod(Type type, string name)
        {
            EnsureComparerType(type);

            return _methods.TryGetValue(type, out var item) && item.Value.IsCompiled(name)
                ? item.Value.GetMethodInfo(name)
                : throw new InvalidOperationException("Compiled method is expected.");
        }

        // exposed compiled type to create instance
        public Type EnsureComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                key => new Lazy<Type>(() => {
                    var info = DefineStaticMethod(key);

                    var compiledComparerType = _buildType(info);

                    foreach (var item in _interfaceMethods) {
                        var method = compiledComparerType.GetMethod(item.Name);
                        info.SetCompiledMethod(method);
                    }

                    return compiledComparerType;
                }));

            var comparerType = lazy.Value;

            FinalizeStartedBuilds();

            return comparerType;
        }

        private StaticMethodsInfo DefineStaticMethod(Type objectType)
        {
            var lazy = _methods.GetOrAdd(objectType,
                key => new Lazy<StaticMethodsInfo>(() => {
                    var typedInterface = typeof(TGenericInterface).MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{typedInterface.FullName}.DynamicComparer",
                        typedInterface);

                    var methodBuilders = _interfaceMethods
                        .Select(method => {
                            var parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

                            var staticMethodParameterTypes = new[] { typeof(IContex) }
                                .Concat(parameterTypes)
                                .Concat(parameterTypes.Select(_ => typeof(CycleDetectionSet)))
                                .ToArray();

                            return typeBuilder.DefineStaticMethod(
                                method.Name,
                                method.ReturnType,
                                staticMethodParameterTypes);
                        })
                        .ToArray();

                    return new StaticMethodsInfo(methodBuilders);
                }));

            return lazy.Value;
        }

        private void FinalizeStartedBuilds()
        {
            foreach (var item in _methods.ToDictionary(x => x.Key, x => x.Value.Value)) {
                if (item.Value.IsCompiled(item.Key.Name)) {
                    continue;
                }

                EnsureComparerType(item.Key);
            }
        }
    }
}
