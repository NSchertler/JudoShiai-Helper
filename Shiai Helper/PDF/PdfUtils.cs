using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace Shiai_Helper.PDF
{
    internal class PdfUtils
    {
        public static Document SetupDocument()
        {
            //if (GlobalFontSettings.FontResolver == null)
            //    GlobalFontSettings.FontResolver = new NewFontResolver();
            var pdf = new Document();

            return pdf;
        }

        public static void SaveToFile(string path, Document pdf)
        {
            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = pdf;
            pdfRenderer.RenderDocument();

            pdfRenderer.PdfDocument.Save(path);
        }
    }
}
