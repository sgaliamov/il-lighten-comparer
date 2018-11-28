using System;
using System.Collections.Generic;

namespace ILLightenComparer
{
    // todo: use expressions
    public sealed class CompareConfiguration
    {
        public HashSet<string> IgnoredMembers { get; set; } = new HashSet<string>();
        public bool IncludeFields { get; set; } = false;
        public string[] MembersOrder { get; set; } = new string[0];
        public StringComparison StringComparisonType { get; set; } = StringComparison.Ordinal;
    }
}
