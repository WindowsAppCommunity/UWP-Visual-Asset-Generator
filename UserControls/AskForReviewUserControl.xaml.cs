using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_Visual_Asset_Generator.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_Visual_Asset_Generator.UserControls
{
    public sealed partial class AskForReviewUserControl : ContentDialog
    {
        public MainViewModel mainViewModel => App.mainViewModel;

        public AskForReviewUserControl()
        {
            this.InitializeComponent();
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.ReviewThisApp();
            Hide();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
