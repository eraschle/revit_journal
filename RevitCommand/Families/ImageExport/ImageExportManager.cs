using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitCommand.Properties;
using System.Collections.Generic;
using System.IO;

namespace RevitCommand.Families.ImageExport
{
    public class ImageExportManager
    {
        private readonly UIDocument UIDocument;
        private readonly Document Document;

        private const string FamilyExtension = ".rfa";

        public ImageExportManager(UIDocument uIDocument)
        {
            UIDocument = uIDocument;
            Document = UIDocument.Document;
        }

        public void CreateNoSymbolImage(string suffix)
        {
            var noSymbol = Resources.no_symbol;
            noSymbol.Save(GetSymboleFileName(suffix));
        }

        public static bool LogoExist(string filePath)
        {
            return File.Exists(filePath);
        }

        //public static void CreateLogo(string filePath)
        //{
        //    var logo = new Bitmap(filePath);
        //    logo.Save(filePath);
        //}


        public void ExportImage(ImageExportOptions options)
        {
            UIDocument.Selection.SetElementIds(new List<ElementId>());
            ZoomToFit();
            Document.ExportImage(options);
        }

        private void ZoomToFit()
        {
            var activeViewId = UIDocument.ActiveView.Id;
            foreach (var uiView in UIDocument.GetOpenUIViews())
            {
                if (uiView.ViewId != activeViewId) { continue; }

                uiView.ZoomToFit();
            }
        }

        public ImageExportOptions DefaultOptions(string suffix)
        {
            return new ImageExportOptions
            {
                FilePath = GetSymboleFileName(suffix),
                ZoomType = ZoomFitType.FitToPage,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_600,
            };
        }

        public ImageExportOptions Get3dOptions(string suffix)
        {
            var options = DefaultOptions(suffix);
            options.ExportRange = ExportRange.VisibleRegionOfCurrentView;
            options.ShadowViewsFileType = ImageFileType.PNG;
            options.PixelSize = 1024;
            return options;
        }

        private string GetSymboleFileName(string suffix)
        {
            var imageFile = Document.PathName.Replace(FamilyExtension, $"_{suffix}.png");
            return imageFile;
        }
    }
}
