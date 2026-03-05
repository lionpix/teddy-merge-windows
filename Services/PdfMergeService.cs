using System;
using System.Collections.Generic;
using System.Linq;
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
                var pagesToExtract = ParsePageRange(doc.PageRange, count);
                
                foreach (int zeroBasedIdx in pagesToExtract)
                {
                    if (zeroBasedIdx >= 0 && zeroBasedIdx < count)
                    {
                        PdfPage page = inputDocument.Pages[zeroBasedIdx];
                        outputDocument.AddPage(page);
                    }
                }
            }
            else if (doc.Extension == ".jpg" || doc.Extension == ".jpeg" || doc.Extension == ".png" || doc.Extension == ".tiff" || doc.Extension == ".tif")
            {
                await ImageToPdfConverter.AppendImageAsPdfPagesAsync(doc.FilePath, doc.PageRange, outputDocument);
            }
        }

        outputDocument.Save(outputFilePath);
    }

    public static IEnumerable<int> ParsePageRange(string rangeString, int maxPages)
    {
        if (string.IsNullOrWhiteSpace(rangeString))
        {
            for (int i = 0; i < maxPages; i++) yield return i;
            yield break;
        }

        var parts = rangeString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var p = part.Trim();
            if (string.IsNullOrEmpty(p)) continue;

            if (p.Contains("-"))
            {
                var split = p.Split('-');
                if (split.Length == 2 && int.TryParse(split[0], out int start) && int.TryParse(split[1], out int end))
                {
                    start = Math.Max(1, start);
                    end = Math.Min(maxPages, end);
                    if (start <= end)
                    {
                        for (int i = start; i <= end; i++) yield return i - 1;
                    }
                    else
                    {
                        for (int i = start; i >= end; i--) yield return i - 1;
                    }
                }
            }
            else if (int.TryParse(p, out int page))
            {
                if (page >= 1 && page <= maxPages)
                {
                    yield return page - 1;
                }
            }
        }
    }
}
