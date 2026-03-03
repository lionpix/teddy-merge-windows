using System.Collections.ObjectModel;
using TeddyMerge.Models;

namespace TeddyMerge.Services;

public class DocumentManager
{
    public ObservableCollection<DocumentItem> Documents { get; } = new ObservableCollection<DocumentItem>();

    public void AddDocument(string path)
    {
        Documents.Add(new DocumentItem(path));
    }

    public void RemoveDocument(DocumentItem item)
    {
        Documents.Remove(item);
    }

    public void MoveDocumentUp(DocumentItem item)
    {
        int index = Documents.IndexOf(item);
        if (index > 0)
        {
            Documents.Move(index, index - 1);
        }
    }

    public void MoveDocumentDown(DocumentItem item)
    {
        int index = Documents.IndexOf(item);
        if (index >= 0 && index < Documents.Count - 1)
        {
            Documents.Move(index, index + 1);
        }
    }

    public void Clear()
    {
        Documents.Clear();
    }
}
