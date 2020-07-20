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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP_Visual_Asset_Generator.UserControls
{
    public sealed partial class AssetBrowserUserControl : UserControl
    {
        public MainViewModel mainViewModel { get; set; }

        public AssetBrowserUserControl()
        {
            this.InitializeComponent();
            mainViewModel = App.mainViewModel;
        }

        private void text_TopMargin_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            int parsedValue;

            if (!int.TryParse(args.NewText, out parsedValue))
            {
                args.Cancel = true;
            }
            else
            {
                var vm = sender.DataContext as AssetViewModel;

                if (parsedValue != 0) //Value of 0 gets a pass, as it is the default.
                {
                    if (vm != null)
                    {
                        if (parsedValue > vm.HalfOf(vm.ImageHeight) ||
                            parsedValue < 0)
                        {
                            args.Cancel = true;
                        }
                    }
                }
            }
        }

        private void text_BottomMargin_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {       
            int parsedValue;

            if (!int.TryParse(args.NewText, out parsedValue))
            {
                args.Cancel= true;
            }
            else
            {
                var vm = sender.DataContext as AssetViewModel;

                if (parsedValue != 0) //Value of 0 gets a pass, as it is the default.
                {
                    if (vm != null)
                    {
                        if (parsedValue > vm.HalfOf(vm.ImageHeight) ||
                            parsedValue < 0)
                        {
                            args.Cancel = true;
                        }
                    }
                }
            }
        }

        private void toggleAllButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleOn = !mainViewModel.AssetTypes.Current.Assets.Items[0].SelectedForExport;
            
            foreach (var element in mainViewModel.AssetTypes.Current.Assets.Items)
            {
                element.SelectedForExport = toggleOn;
            }
            
        }
    }
}
