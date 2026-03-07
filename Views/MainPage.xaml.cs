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
    private readonly Microsoft.Windows.ApplicationModel.Resources.ResourceLoader _resourceLoader = new();

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
        e.DragUIOverride.Caption = _resourceLoader.GetString("DropFilesToAdd");
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

    private void OnPageRangeTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var text = sender.Text;
        if (string.IsNullOrEmpty(text)) return;

        var filtered = new string(text.Where(c => char.IsDigit(c) || c == ',' || c == '-' || c == ' ').ToArray());
        
        if (sender.DataContext is DocumentItem item)
        {
            var max = item.PageCount;
            var newFiltered = string.Empty;
            long currentNum = 0;
            
            foreach (char c in filtered)
            {
                if (char.IsDigit(c))
                {
                    long nextNum = currentNum * 10 + (c - '0');
                    if (nextNum > max) 
                    {
                        continue;
                    }
                    currentNum = nextNum;
                    newFiltered += c;
                }
                else
                {
                    currentNum = 0;
                    newFiltered += c;
                }
            }
            filtered = newFiltered;
        }

        if (text != filtered)
        {
            int pos = sender.SelectionStart;
            int diff = text.Length - filtered.Length;
            sender.Text = filtered;
            sender.SelectionStart = Math.Max(0, pos - diff);
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
        filePicker.FileTypeChoices.Add(_resourceLoader.GetString("PdfDocument"), new List<string>() { ".pdf" });
        
        var firstItemName = System.IO.Path.GetFileNameWithoutExtension(ViewModel.Documents[0].FileName);
        filePicker.SuggestedFileName = $"{firstItemName}{_resourceLoader.GetString("MergedSuffix")}";

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
                    Title = _resourceLoader.GetString("MergeCompleteTitle"),
                    Content = string.Format(_resourceLoader.GetString("MergeCompleteContent"), file.Name),
                    CloseButtonText = _resourceLoader.GetString("OkButton"),
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog errDialog = new ContentDialog
                {
                    Title = _resourceLoader.GetString("MergeFailedTitle"),
                    Content = ex.Message,
                    CloseButtonText = _resourceLoader.GetString("OkButton"),
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
            var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var appDir = System.IO.Path.Combine(localAppData, "TeddyMerge");
            System.IO.Directory.CreateDirectory(appDir);
            
            System.IO.File.WriteAllText(System.IO.Path.Combine(appDir, "settings.txt"), lang);
            
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
            Title = _resourceLoader.GetString("LanguageChangedTitle"),
            Content = _resourceLoader.GetString("LanguageChangedContent"),
            CloseButtonText = _resourceLoader.GetString("OkButton"),
            XamlRoot = xamlRoot
        };
        await dialog.ShowAsync();
    }



    private async void OnAboutClicked(object sender, RoutedEventArgs e)
    {
        var versionString = PackageHelper.GetAppVersion();

        var contentPanel = new StackPanel { Spacing = 10 };
        contentPanel.Children.Add(new TextBlock 
        { 
            Text = _resourceLoader.GetString("AboutContent"), 
            TextWrapping = TextWrapping.Wrap 
        });

        contentPanel.Children.Add(new TextBlock 
        { 
            Text = string.Format(_resourceLoader.GetString("AboutVersion"), versionString), 
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 141, 110, 99)),
            FontSize = 12
        });
        
        var linkButton = new HyperlinkButton
        {
            Content = "www.teddymerge.com",
            NavigateUri = new Uri("https://www.teddymerge.com")
        };
        contentPanel.Children.Add(linkButton);

        ContentDialog dialog = new ContentDialog
        {
            Title = _resourceLoader.GetString("AboutTitle"),
            Content = contentPanel,
            CloseButtonText = _resourceLoader.GetString("OkButton"),
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}
