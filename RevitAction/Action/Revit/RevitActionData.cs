using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitAction.Report;
using System;
using System.Diagnostics;

namespace RevitAction.Action.Revit
{

    public class RevitActionData
    {
        public static RevitActionData Create(ExternalCommandData commandData)
        {
            var data = new RevitActionData();
            try
            {
                data.UiApplication = commandData.Application;
                data.UIDocument = commandData.Application.ActiveUIDocument;

                data.Application = commandData.Application.Application;
                data.Document = commandData.Application.ActiveUIDocument.Document;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            return data;
        }

        #region Revit 

        public UIApplication UiApplication { get; private set; }

        public bool HasUiApplication()
        {
            return HasObject(UiApplication, out _);
        }

        public bool HasUiApplication(out UIApplication uiApplication)
        {
            return HasObject(UiApplication, out uiApplication);
        }

        public Application Application { get; private set; }

        public bool HasApplication()
        {
            return HasObject(Application, out _);
        }

        public bool HasApplication(out Application application)
        {
            return HasObject(Application, out application);
        }

        public UIDocument UIDocument { get; private set; }

        public bool HasUiDocument()
        {
            return HasObject(UIDocument, out _);
        }

        public bool HasUIDocument(out UIDocument uiDocument)
        {
            return HasObject(UIDocument, out uiDocument);
        }

        public Document Document { get; private set; }

        public bool HasDocument(out Document document)
        {
            return HasObject(Document, out document);
        }

        public bool HasDocument()
        {
            return HasObject(Document, out _);
        }

        public bool IsFamilyDocument(out Document document)
        {
            return HasObject(Document, out document) && document.IsFamilyDocument;
        }

        public bool IsProjectDocument(out Document document)
        {
            return IsFamilyDocument(out document) == false;
        }

        private bool HasObject<TObj>(TObj element, out TObj obj)
        {
            obj = element;
            return obj != null;
        }

        #endregion

        #region Loggin

        public SuccessReport Success { get; } = new SuccessReport();

        public ErrorReport Error { get; } = new ErrorReport();
        
        public InfoReport Info { get; } = new InfoReport();
        
        public void SetAction(ITaskAction action)
        {
            Success.SetAction(action);
            Error.SetAction(action);
            Info.SetAction(action);
        }

        #endregion

        public RevitFamilyFile RevitFile
        {
            get
            {
                RevitFamilyFile revitFile = null;
                if (HasDocument(out var document))
                {
                    revitFile = AFile.Create<RevitFamilyFile>(document.PathName);
                }
                return revitFile;
            }
        }
    }
}
