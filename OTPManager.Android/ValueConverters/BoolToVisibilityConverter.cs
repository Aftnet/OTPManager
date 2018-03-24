using Android.Views;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace OTPManager.Android.ValueConverters
{
    public class BoolToVisibilityConverter : MvxValueConverter<bool, ViewStates>
    {
        protected override ViewStates Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}