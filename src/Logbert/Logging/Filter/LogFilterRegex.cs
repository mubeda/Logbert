#region Copyright © 2024 Logbert Contributors

// File:    LogFilterRegex.cs
// Package: Logbert
// Project: Logbert
//
// Copyright (c) 2024, Logbert Contributors. All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.

#endregion

using System;
using System.Text.RegularExpressions;

namespace Com.Logbert.Logging.Filter
{
  /// <summary>
  /// Implements a simple regular expression based <see cref="LogFilter"/> for <see cref="LogMessage"/>s.
  /// </summary>
  public class LogFilterRegex : LogFilterString
  {
    #region Private Fields

    /// <summary>
    /// Holds the regular expression of the filter.
    /// </summary>
    private readonly Regex mFilterRegex;

    /// <summary>
    /// Holds the error message if the regex pattern is invalid.
    /// </summary>
    private readonly string mErrorMessage;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets a value indicating whether the regex pattern is valid.
    /// </summary>
    public bool IsValid => mFilterRegex != null;

    /// <summary>
    /// Gets the error message if the regex pattern is invalid, or null if valid.
    /// </summary>
    public string ErrorMessage => mErrorMessage;

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether the given <paramref name="value"/> matches the filter, or not.
    /// </summary>
    /// <param name="value">The value that may be match the filter.</param>
    /// <returns><c>True</c> if the given <paramref name="value"/> matches the filter, otherwise <c>false</c>.</returns>
    public override bool Match(object value)
    {
      if (mFilterRegex == null || value == null)
      {
        return false;
      }

      try
      {
        return mFilterRegex.IsMatch(value.ToString());
      }
      catch (RegexMatchTimeoutException)
      {
        // Pattern took too long to match, treat as no match
        return false;
      }
    }

    /// <summary>
    /// Validates a regex pattern without creating a filter.
    /// </summary>
    /// <param name="pattern">The regex pattern to validate.</param>
    /// <param name="errorMessage">The error message if invalid, or null if valid.</param>
    /// <returns><c>True</c> if the pattern is valid, otherwise <c>false</c>.</returns>
    public static bool TryValidatePattern(string pattern, out string errorMessage)
    {
      if (string.IsNullOrEmpty(pattern))
      {
        errorMessage = null;
        return true;
      }

      try
      {
        _ = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
        errorMessage = null;
        return true;
      }
      catch (ArgumentException ex)
      {
        errorMessage = ex.Message;
        return false;
      }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of a <see cref="LogFilterRegex"/>.
    /// </summary>
    /// <param name="columnIndex">The column index to apply the filter to.</param>
    /// <param name="value">The regular expression to filter for.</param>
    public LogFilterRegex(int columnIndex, string value) : base(columnIndex, value)
    {
      try
      {
        // Use a timeout to prevent catastrophic backtracking
        mFilterRegex = new Regex(value ?? string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1));
        mErrorMessage = null;
      }
      catch (ArgumentException ex)
      {
        mFilterRegex = null;
        mErrorMessage = $"Invalid regex pattern: {ex.Message}";
        Logbert.Helper.Logger.Warn(mErrorMessage);
      }
    }

    #endregion
  }
}
