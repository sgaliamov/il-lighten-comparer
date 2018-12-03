using System;

namespace ILLightenComparer
{
    // todo: use expressions
    // todo: convert to builder
    /// <summary>
    ///     For not a defined setting a default value will be used.
    /// </summary>
    public sealed class ComparerSettings
    {
        public bool? DetectCycles { get; set; }
        public string[] IgnoredMembers { get; set; }
        public bool? IncludeFields { get; set; }
        public string[] MembersOrder { get; set; }
        public StringComparison? StringComparisonType { get; set; }
    }
}
