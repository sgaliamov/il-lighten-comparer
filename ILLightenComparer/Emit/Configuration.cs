using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ILLightenComparer.Emit
{
    internal sealed class Configuration
    {
        private readonly IDictionary<Type, IComparer> _comparers;

        public Configuration(Configuration configuration) : this(
            configuration.IgnoredMembers,
            configuration.IncludeFields,
            configuration.MembersOrder,
            configuration.StringComparisonType,
            configuration.DetectCycles,
            configuration.IgnoreCollectionOrder,
            configuration._comparers) { }

        public Configuration(
            IEnumerable<string> ignoredMembers,
            bool includeFields,
            IEnumerable<string> membersOrder,
            StringComparison stringComparisonType,
            bool detectCycles,
            bool ignoreCollectionOrder,
            IDictionary<Type, IComparer> comparers)
        {
            if (comparers == null)
            {
                throw new ArgumentNullException(nameof(comparers));
            }

            _comparers = comparers.ToDictionary(x => x.Key, x => x.Value);
            SetIgnoredMembers(ignoredMembers);
            IncludeFields = includeFields;
            SetMembersOrder(membersOrder);
            StringComparisonType = stringComparisonType;
            DetectCycles = detectCycles;
            IgnoreCollectionOrder = ignoreCollectionOrder;
        }

        public bool DetectCycles { get; set; }
        public bool IgnoreCollectionOrder { get; set; }
        public HashSet<string> IgnoredMembers { get; private set; }
        public bool IncludeFields { get; set; }
        public string[] MembersOrder { get; private set; }
        public StringComparison StringComparisonType { get; set; }

        public void SetComparer(Type type, IComparer comparer)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _comparers[type] = comparer;
        }

        public IComparer GetComparer(Type type)
        {
            return _comparers.TryGetValue(type, out var comparer)
                       ? comparer
                       : null;
        }

        public void SetIgnoredMembers(IEnumerable<string> ignoredMembers)
        {
            IgnoredMembers = new HashSet<string>(ignoredMembers ?? throw new ArgumentNullException(nameof(ignoredMembers)));
        }

        public void SetMembersOrder(IEnumerable<string> value)
        {
            MembersOrder = value?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
