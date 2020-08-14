using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCommand.Families
{
    public class SharedParameterManager
    {
        private readonly Application Application;

        public SharedParameterManager(Application application, string filePath)
        {
            Application = application;
            Application.SharedParametersFilename = filePath;
        }

        public IEnumerable<DefinitionGroup> GetGroupDefinitions()
        {
            var sharedParameterFile = Application.OpenSharedParameterFile();
            return sharedParameterFile.Groups.Select(group => group);
        }

        public DefinitionGroup GetGroupDefinitionsByName(string groupName)
        {
            return GetGroupDefinitions()
                .Where(group => IsGroupName(group, groupName))
                .FirstOrDefault();
        }

        private bool IsGroupName(DefinitionGroup group, string groupName)
        {
            return group != null && group.Name.Equals(groupName, StringComparison.CurrentCulture);
        }

        public IList<ExternalDefinition> GetSharedParameters()
        {
            var parameters = new List<ExternalDefinition>();
            foreach (var group in GetGroupDefinitions())
            {
                var groupParameters = GetSharedParametersOfGroup(group);
                parameters.AddRange(groupParameters);
            }
            return parameters;
        }

        public IEnumerable<ExternalDefinition> GetSharedParameters(IEnumerable<string> parameterNames)
        {
            return GetSharedParameters()
                .Where(parameter => parameterNames.Contains(parameter.Name));
        }

        public bool HasSharedParameter(string name, out ExternalDefinition definition)
        {
            definition = GetSharedParameters()
                .Where(parameter => IsParameterName(parameter, name))
                .FirstOrDefault();
            return definition != null;
        }

        private static bool IsParameterName(ExternalDefinition definition, string parameterName)
        {
            return definition.Name.Equals(parameterName, StringComparison.CurrentCulture);
        }



        public IList<ExternalDefinition> GetSharedParametersByGroupName(string groupName)
        {
            var parameters = new List<ExternalDefinition>();
            var group = GetGroupDefinitionsByName(groupName);
            return group is null ? parameters : GetSharedParametersOfGroup(group);
        }

        public static IList<ExternalDefinition> GetSharedParametersOfGroup(DefinitionGroup group)
        {
            var parameters = new List<ExternalDefinition>();
            foreach (var definition in group.Definitions)
            {
                if (!(definition is ExternalDefinition externalDefinition)) { continue; }

                parameters.Add(externalDefinition);
            }
            return parameters;
        }

    }
}
