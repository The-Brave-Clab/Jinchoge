using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Yuyuyui.PrivateServer.GUI.Controls
{
    public partial class LogEntryControl : ContentControl
    {
        public static readonly StyledProperty<Utils.LogType> LogLevelProperty =
            AvaloniaProperty.Register<LogEntryControl, Utils.LogType>(nameof(LogLevel));

        public Utils.LogType LogLevel
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

        public static readonly StyledProperty<string> CreatedTimeProperty =
            AvaloniaProperty.Register<LogEntryControl, string>(nameof(CreatedTime));

        public string CreatedTime
        {
            get => GetValue(CreatedTimeProperty);
            set => SetValue(CreatedTimeProperty, value);
        }

        public LogEntryControl()
        {
            InitializeComponent();
        }
    }



    public class LogTypeToBackgroundBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Utils.LogType logType && targetType.IsAssignableTo(typeof(IBrush)))
            {
                switch (logType)
                {
                    case Utils.LogType.Trace:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x00, 0x00, 0x7f));
                    case Utils.LogType.Info:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x00, 0x7f, 0x00));
                    case Utils.LogType.Warning:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x7f, 0x7f, 0x00));
                    case Utils.LogType.Error:
                        return new SolidColorBrush(Color.FromArgb(0x4f, 0x7f, 0x00, 0x00));
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
            if (value is Utils.LogType logType && targetType.IsAssignableTo(typeof(IBrush)))
            {
                switch (logType)
                {
                    case Utils.LogType.Trace:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0xff));
                    case Utils.LogType.Info:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0xff, 0x99));
                    case Utils.LogType.Warning:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0x99));
                    case Utils.LogType.Error:
                        return new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x99, 0x99));
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
            if (value is Utils.LogType logType && targetType.IsAssignableTo(typeof(string)))
            {
                return logType.ToString("G").ToUpper();
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
