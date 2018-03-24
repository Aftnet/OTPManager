using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using ZXing.Common;
using ZXing.Mobile;

namespace OTPManager.UWP.ValueConverters
{
    public class QRConverter : IValueConverter
    {
        private const int QRSize = 800;
        private static readonly WriteableBitmapRenderer Renderer = new WriteableBitmapRenderer();
        private static readonly EncodingOptions RenderSettings = new EncodingOptions
        {
            Height = QRSize,
            Width = QRSize,
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            var matrix = value as BitMatrix;
            if (matrix == null)
                throw new ArgumentException();

            var output = Renderer.Render(matrix, ZXing.BarcodeFormat.QR_CODE, string.Empty, RenderSettings);
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
