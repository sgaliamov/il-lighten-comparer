using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Visitors
{
    internal interface IVisitor
    {
        void Visit(ILGenerator il, PropertyInfo info);
        void Visit(ILGenerator il, FieldInfo info);
    }
}
