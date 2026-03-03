using System;

namespace TeddyMerge.Models;

public class DocumentItem
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }

    public DocumentItem(string path)
    {
        FilePath = path;
        FileName = System.IO.Path.GetFileName(path);
        Extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
    }
}
