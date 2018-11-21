using System;
using System.Collections.Generic;

namespace ILLightenComparer
{
    public sealed class CompareConfiguration
    {
        public bool IncludeFields { get; set; } = false;
        public StringComparison StringComparisonType { get; set; } = StringComparison.CurrentCulture;
        public HashSet<string> IgnoredMembers { get; set; } = new HashSet<string>();
    }
}
