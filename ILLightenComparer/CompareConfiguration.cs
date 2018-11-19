using System.Collections.Generic;

namespace ILLightenComparer
{
    public sealed class CompareConfiguration
    {
        public bool IncludeFields { get; set; } = false;
        public HashSet<string> IgnoredMembers { get; set; }
    }
}
