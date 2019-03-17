using System;
using System.Collections.Generic;
using System.Linq;

namespace ILLightenComparer.Config
{
    internal sealed class Configuration
    {
        public Configuration(Configuration configuration) : this(
            configuration.IgnoredMembers,
            configuration.IncludeFields,
            configuration.MembersOrder,
            configuration.StringComparisonType,
            configuration.DetectCycles,
            configuration.IgnoreCollectionOrder) { }

        public Configuration(
            IEnumerable<string> ignoredMembers,
            bool includeFields,
            IEnumerable<string> membersOrder,
            StringComparison stringComparisonType,
            bool detectCycles,
            bool ignoreCollectionOrder
        )
        {
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
