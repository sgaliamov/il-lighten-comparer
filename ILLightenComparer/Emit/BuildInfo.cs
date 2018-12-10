using System;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit
{
    internal sealed class BuildInfo : IEquatable<BuildInfo>
    {
        public BuildInfo(Type objectType, TypeBuilder typeBuilder, MethodBuilder methodBuilder)
        {
            ObjectType = objectType;
            TypeBuilder = typeBuilder;
            MethodBuilder = methodBuilder;
        }

        public MethodBuilder MethodBuilder { get; set; }
        public Type ObjectType { get; }
        public TypeBuilder TypeBuilder { get; set; }

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
