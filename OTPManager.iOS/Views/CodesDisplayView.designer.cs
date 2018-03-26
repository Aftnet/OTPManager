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
    [Register ("CodesDisplayView")]
    partial class CodesDisplayView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView ProgressBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProgressBar != null) {
                ProgressBar.Dispose ();
                ProgressBar = null;
            }
        }
    }
}