using System.Linq;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class MembersHasher : IHasherEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly MembersProvider _membersProvider;
        private readonly HasherResolver _resolver;
        private readonly IVariable _variable;

        private MembersHasher(
            HasherResolver resolver,
            MembersProvider membersProvider,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            _variable = variable;
            _resolver = resolver;
            _membersProvider = membersProvider;
            _configuration = configuration;
        }

        public static MembersHasher Create(
            HasherResolver resolver,
            MembersProvider membersProvider,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersHasher(resolver, membersProvider, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            var variableType = _variable.VariableType;

            var comparisons = _membersProvider
                .GetMembers(variableType)
                .Select(_resolver.GetHasher);

            var config = _configuration.Get(variableType);

            il.LoadLong(config.HashSeed)
              .Store(typeof(long), out var seed);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    var add = Add(
                        ShiftLeft(LoadLocal(seed), LoadInteger(5)),
                        LoadLong(config.HashSeed));

                    Xor(add, Execute(item.Emit) + Cast(typeof(long)))(il);
                }
            }

            return il;
        }
    }
}