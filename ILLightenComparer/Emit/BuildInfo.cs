using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit
{
    internal sealed class BuildInfo
    {
        public BuildInfo(Type objectType, MethodInfo method)
        {
            ObjectType = objectType;
            Method = method;
        }

        public bool Compiled => Method.GetType() != typeof(MethodBuilder);
        public MethodInfo Method { get; set; }
        public Type ObjectType { get; }
    }
}
