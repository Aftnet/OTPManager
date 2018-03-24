using MvvmCross.iOS.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS.Views
{
    public partial class AddGeneratorView : MvxViewController<AddGeneratorViewModel>
    {
        public AddGeneratorView() : base("AddGeneratorView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

