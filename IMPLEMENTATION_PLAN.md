# Teddy Merge: Windows Native Implementation Plan

This project is a native Windows Desktop version of **Teddy Merge**, built using **WinUI 3** and **.NET 10**. It is designed specifically for the Windows Store.

## **Core Technology Stack**

- **Framework**: [WinUI 3 (Windows App SDK)](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
- **Runtime**: .NET 10
- **PDF Handling**: [PdfSharp](https://github.com/empira/PDFsharp)
- **Image Handling**: `Windows.Graphics.Imaging` (WinRT APIs) for TIFF/JPG/PNG decoding.

## **Project Structure**

| Feature | Description |
| --- | --- |
| **Logic** | C# backend for file list management and PDF merging. |
| **UI** | XAML-based modern Windows interface with WinUI 3 controls. |
| **Packaging** | MSIX packaging for Windows Store distribution. |

## **Implementation Phases**

### **Phase 1: Project Initialization**
- [ ] Initialize WinUI 3 (Packaged) project.
- [ ] Set up NuGet dependencies for **PdfSharp**.
- [ ] Configure App Manifest for Windows Store requirements.

### **Phase 2: Core PDF & Image Logic**
- [ ] Implement `DocumentManager` to track the list of files and their order.
- [ ] Implement `PdfMergeService` using **PdfSharp**.
- [ ] Implement `ImageToPdfConverter` using `Windows.Graphics.Imaging` for TIFF/PNG/JPG support.

### **Phase 3: User Interface**
- [ ] Create a main window with a `ListView` for document management.
- [ ] Implement **Drag and Drop** support for files from File Explorer.
- [ ] Build the "Merge" action with progress feedback (ProgressRing/ProgressBar).
- [ ] Add item removal and reordering (up/down arrows or drag-reorder).

### **Phase 4: Optimization & Store Ready**
- [ ] Optimize memory usage for large PDF/Image files.
- [ ] Finalize AppIcons and Store branding.
- [ ] Create MSIX package for testing and submission.

## **Verification**
- [ ] Verify merge output against original Flutter version.
- [ ] Test on Windows 10 and 11.
- [ ] Validate MSIX package with Windows App Certification Kit.
