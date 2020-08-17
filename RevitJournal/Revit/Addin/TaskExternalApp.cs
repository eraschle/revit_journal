using Autodesk.Revit.UI;
using System;

namespace RevitJournal.Revit.Addin
{
    public class TaskExternalApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }
    }
}
