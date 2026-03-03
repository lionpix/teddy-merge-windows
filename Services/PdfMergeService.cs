using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using TeddyMerge.Models;

namespace TeddyMerge.Services;

public class PdfMergeService
{
    public async Task MergeDocumentsAsync(IEnumerable<DocumentItem> documents, string outputFilePath)
    {
        using PdfDocument outputDocument = new PdfDocument();

        foreach (var doc in documents)
        {
            if (doc.Extension == ".pdf")
            {
                using PdfDocument inputDocument = PdfReader.Open(doc.FilePath, PdfDocumentOpenMode.Import);
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }
            else if (doc.Extension == ".jpg" || doc.Extension == ".jpeg" || doc.Extension == ".png" || doc.Extension == ".tiff" || doc.Extension == ".tif")
            {
                await ImageToPdfConverter.AppendImageAsPdfPagesAsync(doc.FilePath, outputDocument);
            }
        }

        outputDocument.Save(outputFilePath);
    }
}
