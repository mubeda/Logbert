#region Copyright © 2024 Logbert Contributors

// File:    IOptionPanel.cs
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

using Avalonia.Media;
using Avalonia;

namespace Logbert.Interfaces
{
  /// <summary>
  /// An Interface for all option panel controls to display in the option dialog.
  /// </summary>
  public interface IOptionPanel
  {
    #region Interface Properties

    /// <summary>
    /// Gets the name of the <see cref="IOptionPanel"/> control.
    /// </summary>
    string PanelName
    {
      get;
    }

    /// <summary>
    /// Gets the image to display left of the <see cref="IOptionPanel"/> name.
    /// </summary>
    IImage? Image
    {
      get;
    }

    #endregion

    #region Interface Methods

    /// <summary>
    /// Method will be called before the <see cref="IOptionPanel"/> is shown first.
    /// </summary>
    void InitializeControl();

    /// <summary>
    /// Method will be called before the <see cref="IOptionPanel"/> is closed.
    /// </summary>
    /// <returns><c>True</c> if the <see cref="IOptionPanel"/> may be closed, otherwise <c>false</c>.</returns>
    bool ValidateControl();

    /// <summary>
    /// Method will be called before the <see cref="IOptionPanel"/> is shown.
    /// </summary>
    void BeforePanelShow();

    /// <summary>
    /// Method will be called to adjust the size and location of the panel.
    /// </summary>
    /// <param name="parentBounds">The bounds of the parent container.</param>
    void AdjustSizeAndLocation(Rect parentBounds);

    /// <summary>
    /// Informs the child <see cref="IOptionPanel"/> control about the size change of the parent control.
    /// </summary>
    /// <param name="parentSize">The new size of the parent control.</param>
    void AdjustSize(Size parentSize);

    /// <summary>
    /// Tells the <see cref="IOptionPanel"/> to save the new settings.
    /// </summary>
    void SaveSettings();

    #endregion
  }
}
