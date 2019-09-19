
using UWP_Visual_Asset_Generator;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace MVVM
{
    public class ValleyBaseViewModel : BindableBase
    {

        private bool _thinking;
        private string _thinkingText;

        public bool Thinking
        {
            get
            {
                return _thinking;
            }
            set
            {
                if (_thinking != value)
                {
                    _thinking = value;
                    NotifyPropertyChanged("Thinking");
                }
            }
        }

        public string ThinkingText
        {
            get
            {
                return _thinkingText;
            }
            set
            {
                if (_thinkingText != value)
                {
                    _thinkingText = value;
                    NotifyPropertyChanged("ThinkingText");
                }
            }
        }

        /// <summary>
        /// Standard dialog, to keep consistency.
        /// Long term, better to put this into base viewmodel class, along with MVVM stuff (NotifyProperyCHanged, etc) and inherrit it.
        /// Note that the Secondarytext can be un-assigned, then the econdary button won't be presented.
        /// Result is true if the user presses primary text button
        /// </summary>
        /// <param name="title">
        /// The title of the message dialog
        /// </param>
        /// <param name="message">
        /// THe main body message displayed within the dialog
        /// </param>
        /// <param name="primaryText">
        /// Text to be displayed on the primary button (which returns true when pressed).
        /// If not set, defaults to 'OK'
        /// </param>
        /// <param name="secondaryText">
        /// The (optional) secondary button text.
        /// If not set, it won't be presented to the user at all.
        /// </param>
        public async Task<bool> ShowDialog(string title, string message, string primaryText = "OK", string secondaryText = null)
        {
            bool result = false;

            try
            {
                if (App.rootFrame != null)
                {
                    var d = new ContentDialog();

                    d.Title = title;
                    d.Content = message;
                    d.PrimaryButtonText = primaryText;

                    Windows.UI.Xaml.Media.AcrylicBrush myBrush = new Windows.UI.Xaml.Media.AcrylicBrush();
                    myBrush.BackgroundSource = Windows.UI.Xaml.Media.AcrylicBackgroundSource.Backdrop;
                    myBrush.TintColor = (Color)App.rootFrame.Resources["SystemAccentColor"];
                    myBrush.FallbackColor = Color.FromArgb(255, 202, 24, 37);
                    myBrush.TintOpacity = 0.6;
                    d.Background = myBrush;

                    if (!string.IsNullOrEmpty(secondaryText))
                    {
                        d.SecondaryButtonText = secondaryText;
                    }
                    var dr = await d.ShowAsync();

                    result = (dr == ContentDialogResult.Primary);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return result;

        }


    }
}
