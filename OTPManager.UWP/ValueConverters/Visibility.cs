using MvvmCross.Platforms.Uap.Converters;
using MvvmCross.Plugin.Visibility;

namespace OTPManager.UWP.ValueConverters
{
    public class VisibilityConverter : MvxNativeValueConverter<MvxVisibilityValueConverter>
    {
    }

    public class InverseVisibilityConverter : MvxNativeValueConverter<MvxInvertedVisibilityValueConverter>
    {
    }
}
