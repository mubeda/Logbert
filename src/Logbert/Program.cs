#region Copyright © 2015 Couchcoding

// File:    Program.cs
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
using System.IO.Pipes;
using System.Text;
using Avalonia;
using Couchcoding.Logbert.Helper;
using Couchcoding.Logbert.Properties;

namespace Couchcoding.Logbert;

/// <summary>
/// Implements the main entry point of the application.
/// </summary>
internal static class Program
{
    #region Private Consts

    /// <summary>
    /// Defines the system global name for the named piped, used for inter process communication.
    /// </summary>
    private const string NAMED_PIPED_NAME = "{4A966FCD-17C6-41F9-B9BD-6E491FDCC74C}";

    /// <summary>
    /// Defines the global message to bring the logbert main UI to front.
    /// </summary>
    public const string BRING_TO_FRONT_MSG = "LB_BRING_TO_FRONT";

    #endregion

    #region Public Methods

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Upgrade the user settings if necessary.
            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.SaveSettings();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(
                "Error while upgrading the user settings: {0}",
                ex);
        }

        if (Settings.Default.FrmMainAllowOnlyOneInstance && !TryCreateNamedPipe(NAMED_PIPED_NAME))
        {
            Logger.Info("Another instance of Logbert is already running.");

            try
            {
                // Bring the window of the other instance to front.
                using var anotherLogbertInstance = new NamedPipeClientStream(
                    ".",
                    NAMED_PIPED_NAME,
                    PipeDirection.Out);

                anotherLogbertInstance.Connect(500);
                var message = Encoding.UTF8.GetBytes(BRING_TO_FRONT_MSG);
                anotherLogbertInstance.Write(message, 0, message.Length);

                Logger.Info("Notified running instance to come to front.");

                if (args.Length > 0)
                {
                    // Send the command line arguments to the other instance and exit.
                    using var pipe = new NamedPipeClientStream(
                        ".",
                        NAMED_PIPED_NAME,
                        PipeDirection.Out);

                    pipe.Connect(500);
                    var data = Encoding.UTF8.GetBytes(args[0]);
                    pipe.Write(data, 0, data.Length);

                    Logger.Info("Passing arguments to running instance and exiting.");
                }

                return;
            }
            catch (Exception ex)
            {
                Logger.Error("Error communicating with running instance: " + ex.Message);
                return;
            }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Configures and builds the Avalonia application.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    /// <summary>
    /// Tries to create a named pipe server for single instance detection.
    /// </summary>
    private static bool TryCreateNamedPipe(string pipeName)
    {
        try
        {
            var pipe = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            // If we get here, no other instance is running
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
