using Android.Graphics;
using MvvmCross.Platform.Converters;
using System;
using System.Globalization;
using ZXing.Common;
using ZXing.Mobile;

namespace OTPManager.Android.ValueConverters
{
    public class BitMatrixToQRConverter : MvxValueConverter<BitMatrix, Bitmap>
    {
        private const int QRSize = 300;
        private static readonly BitmapRenderer Renderer = new BitmapRenderer();
        private static readonly EncodingOptions RenderSettings = new EncodingOptions
        {
            
            Height = QRSize,
            Width = QRSize,
            Margin = 0
        };

        protected override Bitmap Convert(BitMatrix value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = Renderer.Render(value, ZXing.BarcodeFormat.QR_CODE, string.Empty, RenderSettings);
            return output;
        }
    }
}
