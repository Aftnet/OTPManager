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
        UIKit.UIButton NoGeneratorsAddManually { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NoGeneratorsAddQR { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView NoGeneratorsStackView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView ProgressBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (NoGeneratorsAddManually != null) {
                NoGeneratorsAddManually.Dispose ();
                NoGeneratorsAddManually = null;
            }

            if (NoGeneratorsAddQR != null) {
                NoGeneratorsAddQR.Dispose ();
                NoGeneratorsAddQR = null;
            }

            if (NoGeneratorsStackView != null) {
                NoGeneratorsStackView.Dispose ();
                NoGeneratorsStackView = null;
            }

            if (ProgressBar != null) {
                ProgressBar.Dispose ();
                ProgressBar = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}