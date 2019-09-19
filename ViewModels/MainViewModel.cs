using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class MainViewModel : ValleyBaseViewModel
    {
        private SettingsViewModel _settings;
        
        private bool _showFeedbackControl = false;
        private bool _showGettingStartedControl = false;
        private bool _showEditCurrentImage = false;

        private StorageFile _originalLogoFile;
        private ImageSource _originalLogoImageSource;

        private const string WinStoreProductID = "9MZ6QRQTDKF2";

        public MainViewModel()
        {
            RegisterCommunicationChannel();
            LogFirstUseMetrics();
        }

        #region public properties

        public bool ShowFeedbackControl
        {
            get
            {
                return _showFeedbackControl;
            }
            set
            {
                _showFeedbackControl = value;
                NotifyPropertyChanged("ShowFeedbackControl");
            }
        }

        public StorageFile OriginalLogoFile
        {
            get
            {
                return _originalLogoFile;
            }
        }
        
        public ImageSource OriginalLogoImageSource
        {
            get
            {
                return _originalLogoImageSource;
            }
            set
            {
                _originalLogoImageSource = value;
                NotifyPropertyChanged("OriginalLogoImageSource");
            }
        }

        public bool ShowGettingStartedControl
        {
            get
            {
                return _showGettingStartedControl;
            }
            set
            {
                _showGettingStartedControl = value;
                NotifyPropertyChanged("ShowGettingStartedControl");
            }
        }
        public bool ShowEditCurrentImage
        {
            get
            {
                return _showEditCurrentImage;
            }
            set
            {
                _showEditCurrentImage = value;
                NotifyPropertyChanged("ShowEditCurrentImage");
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SettingsViewModel();
                }
                return _settings;
            }
            set
            {
                _settings = value;
                NotifyPropertyChanged("Settings");
            }
        }

        #endregion public properties

        #region functions

        /// <summary>
        /// Sets the original image to create assets from.
        /// </summary>
        /// <returns>
        /// Returns a bool if successful
        /// </returns>
        /// <param name="fileToLoad"> a nullable param, if you have a StorageFile pass it here and it will load.  Otherwise it will execute a FilePicker.</param>
        public async Task <bool> LoadOriginalImageFromFileAsync(StorageFile fileToLoad = null)
        {
            var result = false;

            if (fileToLoad == null)
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".png");
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                _originalLogoFile = await picker.PickSingleFileAsync();
            }
            else
            {
                _originalLogoFile = fileToLoad;
            }


            if (_originalLogoFile != null)
            {
                NotifyPropertyChanged("OriginalLogoFile");

                using (var randomAccessStream = await _originalLogoFile.OpenAsync(FileAccessMode.Read))
                {
                    var b = new BitmapImage();
                    await b.SetSourceAsync(randomAccessStream);
                    OriginalLogoImageSource = b;
                }
                result = true;
            }

            return result;
        }

        async public void ReviewThisApp()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductID=" + WinStoreProductID));
        }

        private async Task LogFirstUseMetrics()
        {
            if (SystemInformation.IsFirstRun)
            {
                var t = App.Current.RequestedTheme.ToString();

                Analytics.TrackEvent("OS Theme On First Run is: " + t);
            }
        }

        private async Task RegisterCommunicationChannel()
        {
            if (!Settings.DisableNotifications)
            {
                StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                engagementManager.RegisterNotificationChannelAsync();
            }
        }
        #endregion functions

    }

}
