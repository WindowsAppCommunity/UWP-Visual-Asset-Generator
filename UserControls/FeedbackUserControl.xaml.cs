using UWP_Visual_Asset_Generator.ViewModels;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP_Visual_Asset_Generator.UserControls
{
    public sealed partial class FeedbackUserControl : UserControl
    {
        public MainViewModel mainViewModel { get; set; }

        public FeedbackUserControl()
        {
            this.InitializeComponent();
            mainViewModel = App.mainViewModel;
            DataContext = mainViewModel;
        }

        private void Btn_Review_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.ReviewThisApp();
        }

        private async void Btn_Feedback_Click(object sender, RoutedEventArgs e)
        {
            if (StoreServicesFeedbackLauncher.IsSupported())
            {
                var launcher = StoreServicesFeedbackLauncher.GetDefault();
                await launcher.LaunchAsync();
            }
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.ShowFeedbackControl = false;
        }
    }
}
