namespace RevitAction.Action
{
    public interface IActionParameter
    {
        string Name { get; }

        string Value { get; set; }

        string DefaultValue { get; }

        bool BoolValue { get; }

        ParameterAction Action { get; }
    }
}
