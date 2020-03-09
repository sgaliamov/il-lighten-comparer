using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Shared
{
    internal sealed partial class GenericProvider<TGenericInterface>
    {
        internal sealed class StaticMethodsInfo
        {
            private readonly IReadOnlyDictionary<string, (MethodInfo, bool)> _methods;

            public StaticMethodsInfo(IReadOnlyCollection<MethodBuilder> methods) =>
                _methods = methods.ToDictionary(x => x.Name, x => ((MethodInfo)x, false));

            public bool IsCompiled(string name) =>
                _methods.TryGetValue(name, out var info) && info.Item2;

            public MethodInfo GetMethodInfo(string name) => _methods[name].Item1;

            public void SetCompiledMethod(MethodInfo method)
            {
                var info = _methods[method.Name];
                info.Item1 = method;
                info.Item2 = true;
            }
        }
    }
}
