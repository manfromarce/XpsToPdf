using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using PdfSharp.Xps.XpsModel;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Xps.Rendering;
using FixedDocument = PdfSharp.Xps.XpsModel.FixedDocument;
using FixedPage = PdfSharp.Xps.XpsModel.FixedPage;
using IOPath = System.IO.Path;

namespace PdfSharp.Xps
{
  /// <summary>
  /// Main class that provides the functionallity to convert an XPS file into a PDF file.
  /// </summary>
  public class XpsConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="XpsConverter"/> class.
    /// </summary>
    /// <param name="pdfDocument">The PDF document.</param>
    /// <param name="xpsDocument">The XPS document.</param>
    public XpsConverter(PdfDocument pdfDocument, XpsDocument xpsDocument)
    {
      if (pdfDocument == null)
        throw new ArgumentNullException("pdfDocument");
      if (xpsDocument == null)
        throw new ArgumentNullException("xpsDocument");

      this.pdfDocument = pdfDocument;
      this.xpsDocument = xpsDocument;

      Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XpsConverter"/> class.
    /// </summary>
    /// <param name="pdfDocument">The PDF document.</param>
    /// <param name="xpsDocumentPath">The XPS document path.</param>
    public XpsConverter(PdfDocument pdfDocument, string xpsDocumentPath)  // TODO: a constructor with an Uri
    {
      if (pdfDocument == null)
        throw new ArgumentNullException("pdfDocument");
      if (String.IsNullOrEmpty(xpsDocumentPath))
        throw new ArgumentNullException("xpsDocumentPath");

      this.pdfDocument = pdfDocument;
      xpsDocument = XpsDocument.Open(xpsDocumentPath);

      Initialize();
    }

    void Initialize()
    {
      context = new DocumentRenderingContext(pdfDocument);
    }

    DocumentRenderingContext Context => context;
    DocumentRenderingContext context;

    /// <summary>
    /// Gets the PDF document of this converter.
    /// </summary>
    public PdfDocument PdfDocument => pdfDocument;

    PdfDocument pdfDocument;

    /// <summary>
    /// Gets the XPS document of this converter.
    /// </summary>
    public XpsDocument XpsDocument => xpsDocument;

    XpsDocument xpsDocument;

    /// <summary>
    /// Converts the specified PDF file into an XPS file. The new file is stored in the same directory.
    /// </summary>
    public static void Convert(string xpsFilename)
    {
      if (String.IsNullOrEmpty(xpsFilename))
        throw new ArgumentNullException("xpsFilename");

      if (!File.Exists(xpsFilename))
        throw new FileNotFoundException("File not found.", xpsFilename);

      string pdfFilename = xpsFilename;
      if (IOPath.HasExtension(pdfFilename))
        pdfFilename = pdfFilename.Substring(0, pdfFilename.LastIndexOf('.'));
      pdfFilename += ".pdf";

      Convert(xpsFilename, pdfFilename, 0);
    }

    /// <summary>
    /// Implements the PDF file to XPS file conversion.
    /// </summary>
    public static void Convert(string xpsFilename, string pdfFilename, int docIndex)
    {
            if (String.IsNullOrEmpty(xpsFilename))
                throw new ArgumentNullException("xpsFilename");

            if (String.IsNullOrEmpty(pdfFilename))
            {
                pdfFilename = xpsFilename;
                if (IOPath.HasExtension(pdfFilename))
                    pdfFilename = pdfFilename.Substring(0, pdfFilename.LastIndexOf('.'));
                pdfFilename += ".pdf";
            }

            XpsDocument xpsDocument = null;
            try
            {
                xpsDocument = XpsDocument.Open(xpsFilename);
                PdfDocument pdfDocument = new PdfDocument();
                PdfRenderer renderer = new PdfRenderer();

                int pageIndex = 0;
                foreach (FixedDocument fixedDocument in xpsDocument.Documents)
                    foreach (FixedPage page in fixedDocument.Pages)
                    {
                        if (page == null)
                            continue;
                        Debug.WriteLine(String.Format("  doc={0}, page={1}", docIndex, pageIndex));
                        PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
                        renderer.RenderPage(pdfPage, page);
                        pageIndex++;
                    }
                pdfDocument.Save(pdfFilename);
                xpsDocument.Close();
                xpsDocument = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (xpsDocument != null)
                    xpsDocument.Close();
                throw;
            }
            finally
            {
                if (xpsDocument != null)
                    xpsDocument.Close();
            }
        }

    /// <summary>
    /// Implements the PDF file to XPS file conversion.
    /// </summary>
    public static void Convert(XpsDocument xpsDocument, string pdfFilename, int docIndex)
    {

        if (xpsDocument == null)
            throw new ArgumentNullException("xpsDocument");

        if (String.IsNullOrEmpty(pdfFilename))
            throw new ArgumentNullException("pdfFilename");

        PdfDocument pdfDocument = new PdfDocument();
        PdfRenderer renderer = new PdfRenderer();

        int pageIndex = 0;
        foreach (FixedDocument fixedDocument in xpsDocument.Documents)
        foreach (FixedPage page in fixedDocument.Pages)
        {
            if (page == null)
                continue;
            Debug.WriteLine(String.Format("  doc={0}, page={1}", docIndex, pageIndex));
            PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
            renderer.RenderPage(pdfPage, page);
            pageIndex++;
        }
        pdfDocument.Save(pdfFilename);

    }
  }
}