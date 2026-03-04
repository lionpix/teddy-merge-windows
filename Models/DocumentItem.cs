using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfSharp.Pdf.IO;

namespace TeddyMerge.Models;

public class DocumentItem : INotifyPropertyChanged
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    
    // For UI display like "19 pages"
    public int PageCount { get; set; }

    private string _pageRange;
    public string PageRange
    {
        get => _pageRange;
        set
        {
            if (_pageRange != value)
            {
                _pageRange = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DocumentItem(string path)
    {
        FilePath = path;
        FileName = System.IO.Path.GetFileName(path);
        Extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
        _pageRange = string.Empty;

        // Best effort to get page count during add
        try 
        {
            if (Extension == ".pdf")
            {
                using var pdf = PdfReader.Open(FilePath, PdfDocumentOpenMode.Import);
                PageCount = pdf.PageCount;
            }
            else
            {
                PageCount = 1;
            }
        }
        catch 
        {
            PageCount = 1; // Fallback
        }
    }
}
