﻿using MvvmCross.Uwp.Views;
using OTPManager.Shared.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OTPManager.UWP.Views
{
    public sealed partial class DisplayGeneratorView : MvxWindowsPage
    {
        public DisplayGeneratorViewModel VM => ViewModel as DisplayGeneratorViewModel;

        public DisplayGeneratorView()
        {
            this.InitializeComponent();
        }
    }
}
