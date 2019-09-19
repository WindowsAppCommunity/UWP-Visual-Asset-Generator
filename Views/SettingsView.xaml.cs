using UWP_Visual_Asset_Generator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_Visual_Asset_Generator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public MainViewModel mainViewModel { get; set; }

        public SettingsView()
        {
            this.InitializeComponent();

            mainViewModel = App.mainViewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.rootFrame.BackStack.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
