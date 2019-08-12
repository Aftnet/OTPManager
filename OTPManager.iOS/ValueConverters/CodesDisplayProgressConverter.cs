using MvvmCross.Converters;
using System;
using System.Globalization;

namespace OTPManager.iOS.ValueConverters
{
    public class CodesDisplayProgressConverter : MvxValueConverter<int, float>
    {
        protected override float Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var maxProgress = (int)parameter;
            var output = (float)value / (float)maxProgress;
            return output;
        } 
    }
}
