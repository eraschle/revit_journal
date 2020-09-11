namespace RevitAction.Action
{
    public interface IActionParameter
    {
        /// <summary>
        /// Display name of the parameter
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Default value of the parameter if any
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// The kind of the parameter. Used for displaying in the UI
        /// </summary>
        ParameterKind Kind { get; }

        /// <summary>
        /// The key for the Revit jounral command
        /// </summary>
        string JournalKey { get; }

        bool IsJournalParameter { get; }

        /// <summary>
        /// Return the parameter value as boolean. If the value can not be parsed it will return false
        /// </summary>
        bool GetBoolValue();

        string GetJournalValue();

        void SetJournalValue(string journalValue);
    }
}
