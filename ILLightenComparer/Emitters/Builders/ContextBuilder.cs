using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emitters.Builders
{
    internal sealed class ContextBuilder
    {
        private readonly ConcurrentDictionary<Type, Lazy<BuildInfo>> _builds = new ConcurrentDictionary<Type, Lazy<BuildInfo>>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ModuleBuilder _moduleBuilder;

        public ContextBuilder(ComparerTypeBuilder comparerTypeBuilder)
        {
            _comparerTypeBuilder = comparerTypeBuilder;
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.dll");
        }

        public Type GetOrBuildComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                key => new Lazy<Type>(() =>
                {
                    var buildInfo = GetOrStartBuild(key);

                    var compiledComparerType = _comparerTypeBuilder.Build(
                        (TypeBuilder)buildInfo.CompareMethod.DeclaringType,
                        (MethodBuilder)buildInfo.CompareMethod,
                        buildInfo.ObjectType);

                    var method = compiledComparerType.GetMethod(MethodName.Compare, Method.StaticCompareMethodParameters(buildInfo.ObjectType));

                    buildInfo.FinalizeBuild(method);

                    return compiledComparerType;
                }));

            var comparerType = lazy.Value;

            FinalizeStartedBuilds();

            return comparerType;
        }

        public MethodInfo GetCompiledCompareMethod(Type memberType)
        {
            var comparerType = GetOrBuildComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public BuildInfo GetOrStartBuild(Type objectType)
        {
            var lazy = _builds.GetOrAdd(objectType,
                key => new Lazy<BuildInfo>(() =>
                {
                    var basicInterface = typeof(IComparer);
                    var genericInterface = typeof(IComparer<>).MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{key.FullName}.DynamicComparer",
                        basicInterface,
                        genericInterface);

                    var staticCompareMethodBuilder = typeBuilder.DefineStaticMethod(
                        MethodName.Compare,
                        typeof(int),
                        Method.StaticCompareMethodParameters(key));

                    return new BuildInfo(key, staticCompareMethodBuilder);
                }));

            return lazy.Value;
        }

        private void FinalizeStartedBuilds()
        {
            var builds = _builds.ToArray().Select(x => x.Value).ToArray();

            foreach (var item in builds)
            {
                if (item.Value.Compiled)
                {
                    continue;
                }

                GetOrBuildComparerType(item.Value.ObjectType);
            }
        }
    }
}
