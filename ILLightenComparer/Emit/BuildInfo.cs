using System;
using System.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class BuildInfo : IEquatable<BuildInfo>
    {
        public BuildInfo(Type objectType, Type comparerType, MethodInfo compareMethod)
        {
            ObjectType = objectType;
            ComparerType = comparerType;
            CompareMethod = compareMethod;
        }

        public MethodInfo CompareMethod { get; set; }
        public Type ComparerType { get; set; }
        public bool Compiled { get; set; }
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

        public override int GetHashCode() => ObjectType != null ? ObjectType.GetHashCode() : 0;
    }
}
