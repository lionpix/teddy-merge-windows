# Teddy Merge - Application Description

## Overview
Teddy Merge is a modern Windows desktop application designed to easily merge multiple PDF documents and image files into a single, cohesive PDF file. It provides a simple, intuitive user interface that supports drag-and-drop mechanics, page range selection, and document reordering, making it a valuable productivity tool for document management. The application is developed by LionPixel and is published on the Microsoft Store.

## Core Features
1. **Multi-Format Merging:** Supports merging both PDF documents (`.pdf`) and common image formats (`.jpg`, `.jpeg`, `.png`, `.tiff`, `.tif`) into a single output PDF document. Image files are automatically converted into PDF pages during the merge process.
2. **Page Range Selection:** Users can specify exact page ranges to extract from a multi-page PDF or specific frames from a multi-frame image (like TIFF). It supports formats like `1-3, 5, 8-10`.
3. **Document Management:** 
   - **Drag-and-Drop:** Intuitive drag-and-drop support for quickly adding files into the workspace.
   - **File Ordering:** Easily shift documents up or down to arrange their sequence in the final merged document.
   - **Removal & Clearing:** Ability to selectively remove files from the queue or clear the entire queue simultaneously.
4. **Localization:** Built-in multi-language handling. Language selections are persisted to local application data, allowing users to override the primary application language.
5. **Modern File Pickers:** Uses native Windows `FileOpenPicker` and `FileSavePicker` to ensure a familiar and secure file management experience.

## Technology Stack & Architecture
Teddy Merge is built using modern Microsoft technologies and patterns for Windows client development:

- **Language:** C# 12 / .NET 10.0
- **UI Framework:** WinUI 3 via the Windows App SDK
- **Target OS:** Windows 10 (Min Version 10.0.17763.0, Target Version 10.0.19041.0) and Windows 11
- **Architecture Pattern:** Follows an MVVM (Model-View-ViewModel/Services) structural pattern:
  - **Views:** Found in the `Views` folder, housing the UI logic (e.g., `MainPage.xaml`, `MainPage.xaml.cs`).
  - **Models:** Data objects describing the files to be merged and their corresponding statuses/ranges (e.g., `DocumentItem`).
  - **Services:** Contains the business logic:
    - `DocumentManager`: Manages the state and list of queued documents.
    - `PdfMergeService`: Using `PdfSharp` to extract pages from input PDFs and stitch them into a newly generated output document.
    - `ImageToPdfConverter`: Uses `Windows.Graphics.Imaging.BitmapDecoder` to parse image frames, converts them into a compatible `SoftwareBitmap`, encodes them temporarily, and paints them onto PDF pages via `PdfSharp.Drawing.XGraphics`.
- **Dependencies:**
  - `Microsoft.WindowsAppSDK` (WinUI 3 styling and controls)
  - `Microsoft.Windows.SDK.BuildTools`
  - `PDFsharp` (version 6.2.4) - A powerful .NET library used for creating and processing PDF documents.
- **Packaging and Distribution:** The application uses Single-project MSIX Packaging for clean, safe, and verifiable deployment, optimizing it for Microsoft Store distribution.

## Technical Highlights
- **Image Conversion Flow:** When an image is supplied, `ImageToPdfConverter` processes the `StorageFile` using native Windows `BitmapDecoder`. It accounts for animated or multi-frame formats (useful for TIFF), extracting requested frames via custom page range parsing. The frames are translated to standard memory streams, processed as an `XImage`, and drawn perfectly scaled onto new PDF pages.
- **Page Range Parser:** Custom algorithm implemented inside `PdfMergeService.ParsePageRange(...)` to interpret comma-separated boundaries and hyphens, converting 1-based user input numbers to safely constrained 0-based indexing for internal PdfSharp calls.
- **Localization Integration:** Relies on the `Microsoft.Windows.ApplicationModel.Resources.ResourceLoader` to supply string replacements directly from `.resw` or similar String resources for UI texts, tooltips, dialogs, and exception messages.
