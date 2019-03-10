using System.Reflection;

namespace ILLightenComparer.Emitters.Builders
{
    internal sealed class ComparerInfo
    {
        public ComparerInfo(MethodInfo compareMethod)
        {
            CompareMethod = compareMethod;
        }

        public bool Compiled { get; private set; }
        public MethodInfo CompareMethod { get; private set; }

        public void FinalizeBuild(MethodInfo method)
        {
            CompareMethod = method;
            Compiled = true;
        }
    }
}
