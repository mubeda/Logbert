#region Copyright © 2024 Logbert Contributors

// File:    Browser.cs
// Package: Logbert
// Project: Logbert
//
// The MIT License (MIT)
//
// Copyright (c) 2024 Logbert Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Logbert.Helper
{
  /// <summary>
  /// Implements helper methods to access the system browser.
  /// </summary>
  public static class Browser
  {
    #region Public Methods

    /// <summary>
    /// Opens the system web browser with the specified URI.
    /// Cross-platform implementation for Windows, macOS, and Linux.
    /// </summary>
    /// <param name="url">The URL to open in the system browser.</param>
    /// <returns>True if the browser was opened successfully, false otherwise.</returns>
    public static bool Open(string url)
    {
      if (string.IsNullOrWhiteSpace(url))
        return false;

      try
      {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
          // Windows: Use shell execute
          Process.Start(new ProcessStartInfo
          {
            FileName = url,
            UseShellExecute = true
          });
          return true;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
          // macOS: Use 'open' command
          Process.Start("open", url);
          return true;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
          // Linux: Use 'xdg-open' command
          Process.Start("xdg-open", url);
          return true;
        }
        else
        {
          // Unknown platform - try UseShellExecute as fallback
          Process.Start(new ProcessStartInfo
          {
            FileName = url,
            UseShellExecute = true
          });
          return true;
        }
      }
      catch (Exception)
      {
        // Failed to open browser
        return false;
      }
    }

    /// <summary>
    /// Opens the system web browser with the specified URI.
    /// Legacy overload for WinForms compatibility.
    /// </summary>
    /// <param name="URI">The URI to open in the system browser.</param>
    /// <param name="owner">Ignored - kept for backwards compatibility.</param>
    [Obsolete("Use Open(string url) instead. This overload is kept for backwards compatibility.")]
    public static void Open(string URI, object? owner)
    {
      Open(URI);
    }

    #endregion
  }
}
