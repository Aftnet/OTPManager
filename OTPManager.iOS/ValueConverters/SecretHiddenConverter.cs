using System;
using System.Globalization;
using Foundation;
using MvvmCross.Platform.Converters;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS.ValueConverters
{
    public class SecretHiddenConverter : MvxValueConverter<DisplayGeneratorViewModel, string>
    {
        protected override string Convert(DisplayGeneratorViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.AllowExporting ? value.SecretBase32 : NSBundle.MainBundle.LocalizedString("NotExportable", string.Empty);
        } 
    }
}
