using MvvmCross.Converters;
using MvvmCross.Platforms.Uap.Converters;
using System;
using System.Globalization;
using Windows.UI.Xaml.Media.Imaging;
using ZXing.Common;
using ZXing.Mobile;

namespace OTPManager.UWP.ValueConverters
{
    public class QRConverterInternal : MvxValueConverter<BitMatrix, BitmapSource>
    {
        private const int QRSize = 800;
        private static readonly WriteableBitmapRenderer Renderer = new WriteableBitmapRenderer();
        private static readonly EncodingOptions RenderSettings = new EncodingOptions
        {
            Height = QRSize,
            Width = QRSize,
        };

        protected override BitmapSource Convert(BitMatrix value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = value != null ? Renderer.Render(value, ZXing.BarcodeFormat.QR_CODE, string.Empty, RenderSettings) : null;
            return output;
        }
    }

    public class QRConverter : MvxNativeValueConverter<QRConverterInternal>
    {
    }
}
