using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;
using Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;
using System;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class ReceiverConfigurationDialog : Window
{
    private ILogSettingsCtrl? _settingsControl;

    public ILogProvider? ConfiguredReceiver { get; private set; }

    public ReceiverConfigurationDialog(string receiverType)
    {
        InitializeComponent();

        // Set title based on receiver type
        TitleText.Text = $"Configure {receiverType}";
        SubtitleText.Text = "Configure the settings for this log source";

        // Create the appropriate settings control based on receiver type
        switch (receiverType)
        {
            case "Log4Net File":
                var log4NetViewModel = new Log4NetFileReceiverSettingsViewModel();
                _settingsControl = log4NetViewModel;
                SettingsContent.Content = new Log4NetFileReceiverSettingsView
                {
                    DataContext = log4NetViewModel
                };
                break;

            case "NLog File":
                var nlogViewModel = new NLogFileReceiverSettingsViewModel();
                _settingsControl = nlogViewModel;
                SettingsContent.Content = new NLogFileReceiverSettingsView
                {
                    DataContext = nlogViewModel
                };
                break;

            default:
                // For receivers not yet implemented, show a message
                SettingsContent.Content = new TextBlock
                {
                    Text = $"Configuration for '{receiverType}' is not yet implemented.\n\nThis receiver type will be available in a future update.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Margin = new Avalonia.Thickness(20),
                    FontSize = 14
                };
                break;
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (_settingsControl != null)
        {
            var validation = _settingsControl.ValidateSettings();
            if (!validation.Success)
            {
                // Show error message
                var messageBox = new Window
                {
                    Title = "Validation Error",
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new StackPanel
                    {
                        Margin = new Avalonia.Thickness(20),
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = validation.Message,
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap
                            },
                            new Button
                            {
                                Content = "OK",
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                Padding = new Avalonia.Thickness(30, 8)
                            }
                        }
                    }
                };

                if (messageBox.Content is StackPanel panel && panel.Children[1] is Button okButton)
                {
                    okButton.Click += (s, ev) => messageBox.Close();
                }

                messageBox.ShowDialog(this);
                return;
            }

            ConfiguredReceiver = _settingsControl.GetConfiguredInstance();
        }

        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
