using System;
using System.Collections.Generic;
using System.Linq;

namespace ILLightenComparer.Config
{
    internal struct Configuration
    {
        public readonly HashSet<string> IgnoredMembers;
        public readonly bool IncludeFields;
        public readonly string[] MembersOrder;
        public readonly StringComparison StringComparisonType;
        public readonly bool DetectCycles;

        public Configuration Mutate(ComparerSettings settings)
        {
            return new Configuration(
                settings.IgnoredMembers == null
                    ? IgnoredMembers
                    : new HashSet<string>(settings.IgnoredMembers),
                settings.IncludeFields ?? IncludeFields,
                settings.MembersOrder ?? MembersOrder,
                settings.StringComparisonType ?? StringComparisonType,
                settings.DetectCycles ?? DetectCycles);
        }

        public Configuration(
            HashSet<string> ignoredMembers,
            bool includeFields,
            string[] membersOrder,
            StringComparison stringComparisonType,
            bool detectCycles)
        {
            IgnoredMembers = ignoredMembers ?? throw new ArgumentNullException(nameof(ignoredMembers));
            IncludeFields = includeFields;
            MembersOrder = membersOrder?.Distinct().ToArray()
                           ?? throw new ArgumentNullException(nameof(membersOrder));
            StringComparisonType = stringComparisonType;
            DetectCycles = detectCycles;
        }
    }
}
