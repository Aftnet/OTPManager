﻿using MvvmCross.Platform.Converters;
using System;
using System.Globalization;
using ZXing.Common;
using ZXing.Mobile;
using UIKit;

namespace OTPManager.Android.ValueConverters
{
    public class BitMatrixToQRConverter : MvxValueConverter<BitMatrix, UIImage>
    {
        private const int QRSize = 300;
        private static readonly BitmapRenderer Renderer = new BitmapRenderer();
        private static readonly EncodingOptions RenderSettings = new EncodingOptions
        {
            
            Height = QRSize,
            Width = QRSize,
            Margin = 0
        };

        protected override UIImage Convert(BitMatrix value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = Renderer.Render(value, ZXing.BarcodeFormat.QR_CODE, string.Empty, RenderSettings);
            return output;
        }
    }
}
