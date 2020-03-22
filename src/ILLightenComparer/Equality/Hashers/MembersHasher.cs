using System.Linq;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class MembersHasher : IHasherEmitter
    {
        private readonly MembersProvider _membersProvider;
        private readonly HasherResolver _resolver;
        private readonly IVariable _variable;

        private MembersHasher(
            HasherResolver resolver,
            MembersProvider membersProvider,
            IVariable variable)
        {
            _variable = variable;
            _resolver = resolver;
            _membersProvider = membersProvider;
        }

        public static MembersHasher Create(
            HasherResolver resolver,
            MembersProvider membersProvider,
            IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersHasher(resolver, membersProvider, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            var variableType = _variable.VariableType;

            var comparisons = _membersProvider
                .GetMembers(variableType)
                .Select(_resolver.GetHasher);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    item.Emit(il);
                }
            }

            return il;
        }
    }
}