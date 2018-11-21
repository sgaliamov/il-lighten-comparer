using System.Reflection;

namespace ILLightenComparer.Emit.Members.Base
{
    internal abstract class PropertyMember : Member
    {
        public MethodInfo GetterMethod { get; set; }
    }
}
