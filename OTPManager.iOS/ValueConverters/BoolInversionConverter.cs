using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace OTPManager.iOS.ValueConverters
{
    public class BoolInversionConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}
