using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Data;
using Avalonia.Media;
using System.Globalization;
using System;
using Avalonia;

namespace Yuyuyui.PrivateServer.GUI.Controls
{
    public partial class LogEntryControl : ContentControl
    {
        public enum LogType
        {
            Trace,
            Log,
            Warning,
            Error
        }

        public static readonly StyledProperty<LogType> LogLevelProperty =
            AvaloniaProperty.Register<LogEntryControl, LogType>(nameof(LogLevel));

        public LogType LogLevel
        {
            get => GetValue(LogLevelProperty);
            set => SetValue(LogLevelProperty, value);
        }

        public static readonly StyledProperty<string> LogContentProperty =
            AvaloniaProperty.Register<LogEntryControl, string>(nameof(LogContent));

        public string LogContent
        {
            get => GetValue(LogContentProperty);
            set => SetValue(LogContentProperty, value);
        }

        public static readonly DirectProperty<LogEntryControl, string> CreatedTimeProperty =
            AvaloniaProperty.RegisterDirect<LogEntryControl, string>(
                nameof(CreatedTime),
                o => o.CreatedTime);

        private string createdTime = "";

        public string CreatedTime
        {
            get => createdTime;
            private set => SetAndRaise(CreatedTimeProperty, ref createdTime, value);
        }

        public LogEntryControl()
        {
            InitializeComponent();

            CreatedTime = $"[{DateTime.Now:HH:mm:ss}]";
        }
    }



    public class LogTypeToBackgroundBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LogEntryControl.LogType logType && targetType.IsAssignableTo(typeof(IBrush)))
            {
                switch (logType)
                {
                    case LogEntryControl.LogType.Trace:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x00, 0x00, 0x7f));
                    case LogEntryControl.LogType.Log:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x00, 0x7f, 0x00));
                    case LogEntryControl.LogType.Warning:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x7f, 0x7f, 0x00));
                    case LogEntryControl.LogType.Error:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x7f, 0x00, 0x00));
                    default:
                        // invalid option, return the exception below
                        break;
                }
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class LogTypeToBorderBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LogEntryControl.LogType logType && targetType.IsAssignableTo(typeof(IBrush)))
            {
                switch (logType)
                {
                    case LogEntryControl.LogType.Trace:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0xff));
                    case LogEntryControl.LogType.Log:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0xff, 0x99));
                    case LogEntryControl.LogType.Warning:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0x99));
                    case LogEntryControl.LogType.Error:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x99, 0x99));
                    default:
                        // invalid option, return the exception below
                        break;
                }
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class LogTypeToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LogEntryControl.LogType logType && targetType.IsAssignableTo(typeof(string)))
            {
                return logType.ToString("G");
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
