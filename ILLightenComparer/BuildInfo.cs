using System;
using System.Reflection;

namespace ILLightenComparer
{
    internal sealed class BuildInfo
    {
        public BuildInfo(Type objectType, MethodInfo method)
        {
            ObjectType = objectType;
            Method = method;
        }

        public bool Compiled { get; private set; }
        public MethodInfo Method { get; private set; }
        public Type ObjectType { get; }

        public void FinalizeBuild(MethodInfo method)
        {
            Method = method;
            Compiled = true;
        }
    }
}
