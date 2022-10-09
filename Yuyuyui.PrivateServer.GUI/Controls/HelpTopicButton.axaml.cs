using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yuyuyui.PrivateServer.GUI.Controls;

public partial class HelpTopicButton : ContentControl
{
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<HelpTopicButton, ICommand>(nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public static readonly StyledProperty<string> ButtonTextProperty =
        AvaloniaProperty.Register<HelpTopicButton, string>(nameof(ButtonText));

    public string ButtonText
    {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public HelpTopicButton()
    {
        InitializeComponent();
    }
}