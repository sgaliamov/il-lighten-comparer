using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private readonly ComparerContext _context;
        private readonly Func<MemberInfo, IAcceptor>[] _fieldFactories;
        private readonly Func<MemberInfo, IAcceptor>[] _propertyFactories;

        public MemberConverter(
            ComparerContext context,
            Func<MemberInfo, IAcceptor>[] propertyFactories,
            Func<MemberInfo, IAcceptor>[] fieldFactories)
        {
            _context = context;
            _propertyFactories = propertyFactories;
            _fieldFactories = fieldFactories;
        }

        public IAcceptor Convert(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                var acceptor = Convert(memberInfo, _propertyFactories);
                if (acceptor != null)
                {
                    return acceptor;
                }
            }

            var includeFields = _context.GetConfiguration(memberInfo.DeclaringType).IncludeFields;
            if (includeFields && memberInfo is FieldInfo)
            {
                var acceptor = Convert(memberInfo, _fieldFactories);
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
