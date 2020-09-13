﻿using RevitAction;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TaskAppStatus status))
            {
                return string.Empty;
            }

            if (status.IsInitial)
            {
                return "Initial";
            }
            if (status.IsWaiting)
            {
                return "Wait";
            }
            if (status.IsStarted)
            {
                return "Start";
            }
            if (status.IsRunning)
            {
                return "Run";
            }
            if (status.IsError)
            {
                return "Error";
            }
            if (status.IsTimeout)
            {
                return "Time";
            }
            if (status.IsCancel)
            {
                return "Cancel";
            }
            return "Finish";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
