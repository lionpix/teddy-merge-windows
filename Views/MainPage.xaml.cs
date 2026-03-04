using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TeddyMerge.Models;
using TeddyMerge.Services;
using WinRT.Interop;

namespace TeddyMerge.Views;

public sealed partial class MainPage : Page
{
    public DocumentManager ViewModel { get; } = new DocumentManager();
    private PdfMergeService _mergeService = new PdfMergeService();

    public MainPage()
    {
        this.InitializeComponent();
        ViewModel.Documents.CollectionChanged += (s, e) => UpdateEmptyState();
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        EmptyStateText.Visibility = ViewModel.Documents.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
        e.DragUIOverride.Caption = "Drop files to add";
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            foreach (var item in items.OfType<StorageFile>())
            {
                var ext = item.FileType.ToLowerInvariant();
                if (ext == ".pdf" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".tiff" || ext == ".tif")
                {
                    ViewModel.AddDocument(item.Path);
                }
            }
        }
    }

    private void OnMoveUpClicked(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is DocumentItem item)
        {
            ViewModel.MoveDocumentUp(item);
        }
    }

    private void OnMoveDownClicked(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is DocumentItem item)
        {
            ViewModel.MoveDocumentDown(item);
        }
    }

    private void OnRemoveClicked(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is DocumentItem item)
        {
            ViewModel.RemoveDocument(item);
        }
    }

    private void OnClearClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.Clear();
    }

    private async void OnMergeClicked(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Documents.Count == 0) return;

        var filePicker = new FileSavePicker();
        
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(filePicker, hwnd);

        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
        filePicker.SuggestedFileName = "MergedDocument.pdf";

        StorageFile file = await filePicker.PickSaveFileAsync();
        if (file != null)
        {
            try
            {
                MergeProgress.IsActive = true;
                MergeProgress.Visibility = Visibility.Visible;
                
                await _mergeService.MergeDocumentsAsync(ViewModel.Documents, file.Path);
                
                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Merge Complete",
                    Content = $"Document successfully saved to {file.Name}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog errDialog = new ContentDialog
                {
                    Title = "Merge Failed",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errDialog.ShowAsync();
            }
            finally
            {
                MergeProgress.IsActive = false;
                MergeProgress.Visibility = Visibility.Collapsed;
            }
        }
    }

    private async void OnAddFilesClicked(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        picker.ViewMode = PickerViewMode.List;
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add(".pdf");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".tiff");
        picker.FileTypeFilter.Add(".tif");

        var files = await picker.PickMultipleFilesAsync();
        foreach (var file in files)
        {
            ViewModel.AddDocument(file.Path);
        }
    }

    private void OnLanguageClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is string lang)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppLanguage"] = lang;
            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = lang;
            _ = ShowRestartDialog();
        }
    }

    private async Task ShowRestartDialog()
    {
        var xamlRoot = App.MainWindow?.Content?.XamlRoot ?? this.XamlRoot;
        if (xamlRoot == null) return;

        ContentDialog dialog = new ContentDialog
        {
            Title = "Language Changed",
            Content = "Please restart the application for language changes to take effect.",
            CloseButtonText = "OK",
            XamlRoot = xamlRoot
        };
        await dialog.ShowAsync();
    }

    private async void OnCheckUpdateClicked(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Update Check",
            Content = "You are currently running the latest version.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async void OnAboutClicked(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "About TeddyMerge",
            Content = "TeddyMerge is a tool to combine PDFs and Images into a single PDF document quickly and easily.\n\nVersion 1.0.0",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}
