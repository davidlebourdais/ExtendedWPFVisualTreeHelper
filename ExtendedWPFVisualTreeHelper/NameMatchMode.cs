namespace EMA.ExtendedWPFVisualTreeHelper
{
    /// <summary>
    /// Defines how a supplied name filter is interpreted.
    /// </summary>
    public enum NameMatchMode
    {
        /// <summary>
        /// Tries an exact match first, then interprets the filter as a regular expression.
        /// </summary>
        ExactOrRegex,

        /// <summary>
        /// Matches only the exact element name.
        /// </summary>
        Exact,

        /// <summary>
        /// Interprets the filter as a regular expression.
        /// </summary>
        Regex
    }
}
