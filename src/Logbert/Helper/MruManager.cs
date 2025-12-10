#region Copyright © 2015 Couchcoding

// File:    MruManager.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2015 Couchcoding
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchcoding.Logbert.Services;

namespace Couchcoding.Logbert.Helper
{
  /// <summary>
  /// Implements a class to manage the recently used files.
  /// </summary>
  public static class MruManager
  {
    #region Private Consts

    /// <summary>
    /// Defines the maximum count of MRU file.
    /// </summary>
    private const int MAX_MRU_FILE_COUNT = 9;

    #endregion

    #region Public Events

    /// <summary>
    /// Occurs if the MRU list has been changed.
    /// </summary>
    public static event EventHandler? MruListChanged;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the list of all recently used files.
    /// </summary>
    public static List<string> MruFiles => SettingsService.Instance.Settings.RecentFiles ?? new List<string>();

      #endregion

    #region Public Methods

    /// <summary>
    /// Adds the given <paramref name="filename"/> to the recently used files list.
    /// </summary>
    /// <param name="filename">The full path of the file to add.</param>
    public static void AddFile(string filename)
    {
      if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
      {
        SettingsService.Instance.UpdateSettings(settings =>
        {
          settings.RecentFiles ??= new List<string>();

          // Remove existing entry if present
          if (settings.RecentFiles.Contains(filename))
          {
            settings.RecentFiles.Remove(filename);
          }

          // Add to the beginning of the list (most recent first)
          settings.RecentFiles.Insert(0, filename);

          // Ensure maximum count is not exceeded
          while (settings.RecentFiles.Count > MAX_MRU_FILE_COUNT)
          {
            settings.RecentFiles.RemoveAt(settings.RecentFiles.Count - 1);
          }
        });

        SettingsService.Instance.Save();
        MruListChanged?.Invoke(null, EventArgs.Empty);
      }
    }

      /// <summary>
      /// Removes the given <paramref name="filename"/> from the recently used files list.
      /// </summary>
      /// <param name="filename">The full path of the file to remove.</param>
      public static void RemoveFile(string filename)
      {
          if (string.IsNullOrEmpty(filename))
          {
              return;
          }

          SettingsService.Instance.UpdateSettings(settings =>
          {
              settings.RecentFiles?.Remove(filename);
          });

          SettingsService.Instance.Save();
          MruListChanged?.Invoke(null, EventArgs.Empty);
      }

      /// <summary>
    /// Removes all recently used files from the list.
    /// </summary>
    public static void ClearFiles()
    {
      SettingsService.Instance.UpdateSettings(settings =>
      {
          settings.RecentFiles?.Clear();
      });

      SettingsService.Instance.Save();
      MruListChanged?.Invoke(null, EventArgs.Empty);
        }

    #endregion
  }
}
