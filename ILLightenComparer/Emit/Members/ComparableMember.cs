using System.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class ComparableMember : Member
    {
        public MethodInfo CompareToMethod { get; set; }
    }
}
