// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace OTPManager.iOS
{
    [Register ("CodesDisplayItemView")]
    partial class CodesDisplayItemView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel IssuerDisplay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelDisplay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OTPDisplay { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (IssuerDisplay != null) {
                IssuerDisplay.Dispose ();
                IssuerDisplay = null;
            }

            if (LabelDisplay != null) {
                LabelDisplay.Dispose ();
                LabelDisplay = null;
            }

            if (OTPDisplay != null) {
                OTPDisplay.Dispose ();
                OTPDisplay = null;
            }
        }
    }
}