using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Illuminator.Extensions;

namespace ILLightenComparer.Shared
{
    internal sealed class GenericProvider
    {
        private static MethodInfo[] _interfaceMethods;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<StaticMethodsInfo>> _methods = new ConcurrentDictionary<Type, Lazy<StaticMethodsInfo>>();
        private readonly Type _genericInterface;
        private readonly Type _contextType;
        private readonly Func<StaticMethodsInfo, Type, Type> _buildType;

        public GenericProvider(Type genericInterface, Type contextType, Func<StaticMethodsInfo, Type, Type> buildType)
        {
            _buildType = buildType;
            _genericInterface = genericInterface;
            _contextType = contextType;
            _interfaceMethods = genericInterface.GetMethods();
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

                    var compiledComparerType = _buildType(info, key);

                    foreach (var item in _interfaceMethods) {
                        var method = compiledComparerType.GetMethod(item.Name, BindingFlags.Public | BindingFlags.Static);
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
                    var typedInterface = _genericInterface.MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{typedInterface.FullName}.DynamicComparer",
                        typedInterface);

                    var methodBuilders = typedInterface
                        .GetMethods()
                        .Select(method => {
                            var parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

                            var staticMethodParameterTypes = new[] { _contextType }
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
                if (item.Value.AllCompiled()) {
                    continue;
                }

                EnsureComparerType(item.Key);
            }
        }
    }
}
