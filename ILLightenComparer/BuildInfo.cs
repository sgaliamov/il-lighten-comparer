using System;
using System.Reflection;

namespace ILLightenComparer
{
    internal sealed class BuildInfo
    {
        public BuildInfo(Type objectType, MethodInfo compareMethod)
        {
            ObjectType = objectType;
            CompareMethod = compareMethod;
        }

        public bool Compiled { get; private set; }
        public MethodInfo CompareMethod { get; private set; }
        public Type ObjectType { get; }

        public void FinalizeBuild(MethodInfo method)
        {
            CompareMethod = method;
            Compiled = true;
        }
    }
}
