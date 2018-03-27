﻿using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS.ValueConverters
{
    public class CodesDisplayProgressConverter : MvxValueConverter<int, float>
    {
        protected override float Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = (float)value / (float)parameter;
            return output;
        } 
    }
}
