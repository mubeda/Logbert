#region Copyright © 2024 Logbert Contributors

// File:    StringExtensions.cs
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
using System.Text.RegularExpressions;

namespace Logbert.Helper
{
  /// <summary>
  /// Implements string extension methods without WinForms dependencies.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Converts the given <paramref name="baseValue"/> to a CSV compatible string.
    /// </summary>
    /// <param name="baseValue">The <see cref="IComparable"/> type to convert to be CSV compatible.</param>
    /// <returns>A CSV compatible string.</returns>
    public static string ToCsv(this IComparable baseValue)
    {
      return baseValue.ToString().Replace("\"", "\"\"");
    }

    /// <summary>
    /// Converts a wildcard to a <see cref="Regex"/>.
    /// </summary>
    /// <param name="pattern">The wildcard pattern to convert.</param>
    /// <returns>A <see cref="Regex"/> equivalent of the given wildcard <paramref name="pattern"/>.</returns>
    public static string ToRegex(this string pattern)
    {
      if (string.IsNullOrEmpty(pattern))
      {
        return string.Empty;
      }

      return Regex.Escape(pattern).
        Replace("\\*", ".*").
        Replace("\\?", ".");
    }
  }
}
