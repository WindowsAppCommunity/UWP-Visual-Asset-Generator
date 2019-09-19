using UWP_Visual_Asset_Generator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainView : Page
    {
        public MainViewModel mainViewModel { get; set; }

        public MainView()
        {
            this.InitializeComponent();

            mainViewModel = App.mainViewModel;

            DataContext = mainViewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.rootFrame.BackStack.Clear();

            animationOpacityPulse.Begin();
            StartShimmerOnLoadingControl();
        }

        private void StartShimmerOnLoadingControl()
        {
            //get interop compositor
            _compositor = ElementCompositionPreview.GetElementVisual(tbLoadingText).Compositor;

            //get interop visual for XAML TextBlock
            var text = ElementCompositionPreview.GetElementVisual(tbLoadingText);

            _pointLight = _compositor.CreatePointLight();
            _pointLight.Color = Colors.LightPink;
            _pointLight.CoordinateSpace = text; //set up co-ordinate space for offset
            _pointLight.Targets.Add(text); //target XAML TextBlock

            //starts out to the left; vertically centered; light's z-offset is related to fontsize
            _pointLight.Offset = new Vector3(-(float)tbLoadingText.ActualWidth, (float)tbLoadingText.ActualHeight / 2, (float)tbLoadingText.FontSize);

            //simple offset.X animation that runs forever
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1, 2 * (float)tbLoadingText.MinWidth);
            animation.Duration = TimeSpan.FromSeconds(5);
            animation.IterationBehavior = AnimationIterationBehavior.Forever;

            _pointLight.StartAnimation("Offset.X", animation);
        }

        private Compositor _compositor;
        private PointLight _pointLight;

        private void abb_Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.rootFrame.Navigate(typeof(SettingsView), null);
        }

        private void btn_Feedback_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainViewModel.ShowFeedbackControl = true;
        }

        private void Hmi_ShowGettingStartedControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainViewModel.ShowGettingStartedControl = true;
        }

        private void Hmi_ShowWelcomeUSerControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainViewModel.Settings.ShowWelcomePage = true;
            mainViewModel.Settings.ShowUpdatePage = true;
        }
    }
}
