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
        /// <summary>
        ///     Default: true.
        /// </summary>
        public bool? DetectCycles { get; set; }

        /// <summary>
        ///     Default: false.
        /// </summary>
        public bool? IgnoreCollectionOrder { get; set; }

        /// <summary>
        ///     Default: empty.
        /// </summary>
        public string[] IgnoredMembers { get; set; }

        /// <summary>
        ///     Default: true.
        /// </summary>
        public bool? IncludeFields { get; set; }

        /// <summary>
        ///     Default: empty.
        /// </summary>
        public string[] MembersOrder { get; set; }

        /// <summary>
        ///     Default: Ordinal.
        /// </summary>
        public StringComparison? StringComparisonType { get; set; }
    }
}
