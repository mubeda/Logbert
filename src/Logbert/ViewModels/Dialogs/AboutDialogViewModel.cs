using System;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the About dialog.
/// </summary>
public partial class AboutDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _applicationName = "Logbert";

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private string _copyright = "Copyright © 2015 Couchcoding";

    [ObservableProperty]
    private string _description = "Cross-platform log file viewer for various log formats";

    [ObservableProperty]
    private string _licenseText = string.Empty;

    public AboutDialogViewModel()
    {
        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = assembly.GetName().Version;
        Version = versionInfo != null
            ? $"Version {versionInfo.Major}.{versionInfo.Minor}.{versionInfo.Build}"
            : "Version 1.0.0";

        // Load MIT license text
        LicenseText = @"The MIT License (MIT)

Copyright (c) 2015 Couchcoding

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.";
    }
}
