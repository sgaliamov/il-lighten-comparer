using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private static readonly Func<MemberInfo, IAcceptor>[] PropertyFactories =
        {
            StringPropertyMember.Create,
            IntegralPropertyMember.Create,
            NullablePropertyMember.Create,
            BasicPropertyMember.Create,
            HierarchicalPropertyMember.Create
        };

        private static readonly Func<MemberInfo, IAcceptor>[] FieldFactories =
        {
            StringFieldMember.Create,
            IntegralFieldMember.Create,
            NullableFieldMember.Create,
            BasicFieldMember.Create,
            HierarchicalFieldMember.Create
        };

        private readonly Context _context;

        public MemberConverter(Context context) => _context = context;

        public IAcceptor Convert(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                var acceptor = Convert(memberInfo, PropertyFactories);
                if (acceptor != null)
                {
                    return acceptor;
                }
            }

            var includeFields = _context.GetConfiguration(memberInfo.DeclaringType).IncludeFields;
            if (includeFields && memberInfo is FieldInfo)
            {
                var acceptor = Convert(memberInfo, FieldFactories);
                if (acceptor != null)
                {
                    return acceptor;
                }
            }

            throw new NotSupportedException($"{memberInfo.DisplayName()} is not supported.");
        }

        private static IAcceptor Convert(
            MemberInfo memberInfo,
            IEnumerable<Func<MemberInfo, IAcceptor>> factories) =>
            factories
                .Select(factory => factory(memberInfo))
                .FirstOrDefault(acceptor => acceptor != null);
    }
}
