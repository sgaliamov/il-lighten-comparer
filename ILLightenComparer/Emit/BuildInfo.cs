using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit
{
    internal sealed class BuildInfo : IEquatable<BuildInfo>
    {
        public BuildInfo(Type objectType, MethodInfo method)
        {
            ObjectType = objectType;
            Method = method;
        }

        public bool Compiled => Method.GetType() != typeof(MethodBuilder);
        public MethodInfo Method { get; set; }
        public Type ObjectType { get; }

        public bool Equals(BuildInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ObjectType == other.ObjectType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is BuildInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ObjectType != null ? ObjectType.GetHashCode() : 0;
        }
    }
}
