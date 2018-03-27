// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace OTPManager.iOS.Views
{
    [Register ("CodesDisplayItemView")]
    partial class CodesDisplayItemView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Issuer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OTP { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Issuer != null) {
                Issuer.Dispose ();
                Issuer = null;
            }

            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }

            if (OTP != null) {
                OTP.Dispose ();
                OTP = null;
            }
        }
    }
}