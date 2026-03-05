using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace TeddyMerge.Services;

public static class ImageToPdfConverter
{
    public static async Task AppendImageAsPdfPagesAsync(string imagePath, string pageRange, PdfDocument outputDocument)
    {
        StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
        using var stream = await file.OpenReadAsync();
        
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        uint frameCount = decoder.FrameCount;

        var pagesToExtract = PdfMergeService.ParsePageRange(pageRange, (int)frameCount);

        foreach (int i in pagesToExtract)
        {
            if (i < 0 || i >= frameCount) continue;

            BitmapFrame frame = await decoder.GetFrameAsync((uint)i);
            
            using var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, memStream);
            
            SoftwareBitmap softwareBitmap = await frame.GetSoftwareBitmapAsync();
            encoder.SetSoftwareBitmap(softwareBitmap);
            await encoder.FlushAsync();
            
            using Stream dotNetStream = memStream.AsStreamForRead();
            XImage xImage = XImage.FromStream(dotNetStream);
            
            PdfPage page = outputDocument.AddPage();
            // Convert pixels to points (1 point = 1/72 inch). Assume 96 DPI for scaling,
            // or just use XImage PointWidth and PointHeight which PdfSharp calculates.
            page.Width = xImage.PointWidth;
            page.Height = xImage.PointHeight;
            
            using XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.DrawImage(xImage, 0, 0, page.Width.Point, page.Height.Point);
            
            softwareBitmap.Dispose();
        }
    }
}
