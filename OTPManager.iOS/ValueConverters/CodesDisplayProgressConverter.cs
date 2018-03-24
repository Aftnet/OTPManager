using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS.ValueConverters
{
    public class CodesDisplayProgressConverter : MvxValueConverter<CodesDisplayViewModel, float>
    {
        protected override float Convert(CodesDisplayViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            return (float)value.Progress / (float)value.ProgressScale;
        } 
    }
}
