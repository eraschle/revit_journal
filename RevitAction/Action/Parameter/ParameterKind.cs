namespace RevitAction.Action
{
    public enum ParameterKind
    {
        /// <summary>
        /// Hidden parameter in the UI
        /// </summary>
        Hidden,
        /// <summary>
        /// Display changes of the other parameters in the action
        /// </summary>
        InfoDynamic, 
        /// <summary>
        /// Parameter to show read only informations
        /// </summary>
        TextInfoValue, 
        /// <summary>
        /// Parameter to store a string value
        /// </summary>
        TextValue, 
        /// <summary>
        /// A boolean parameter for displaying checkboxes
        /// </summary>
        Boolean, 
        /// <summary>
        /// Parameter to import a text file
        /// </summary>
        TextFile, 
        /// <summary>
        /// Parameter to import a image file
        /// </summary>
        ImageFile, 
        /// <summary>
        /// Parameter to define a specific folder 
        /// </summary>
        SelectFolder, 
        /// <summary>
        /// Unkown
        /// </summary>
        List, 
        /// <summary>
        /// Unkown
        /// </summary>
        Selectable
    }
}
