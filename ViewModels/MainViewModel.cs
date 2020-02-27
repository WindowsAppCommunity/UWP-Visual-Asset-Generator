using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using MVVM;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI;
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
        private bool _previewWithAccentColour = false;
        private bool _showBackgroundColorSelector = false;
        private bool _useTransparentBackground = true;
        private Color _backgroundColour = Color.FromArgb(0, 0, 0, 0);

        public const string mruOutputFolderMetadata = "OutputFolder";

        private StorageFile _originalLogoFile;
        private ImageSource _originalLogoImageSource;
        private StorageFolder _outputFolder;
        public WriteableBitmap originalWriteableBitmap;

        private AssetTypeListViewModel _assetTypes;

        private const string WinStoreProductID = "9MZ6QRQTDKF2";


        private ObservableCollection<KeyValuePair<string, IResampler>> _resamplers = new ObservableCollection<KeyValuePair<string, IResampler>>()
        { new KeyValuePair<string, IResampler>("Average",KnownResamplers.Box),
          new KeyValuePair<string, IResampler>("Bicubic", KnownResamplers.Bicubic ),
          new KeyValuePair<string, IResampler>("Nearest Neighbour", KnownResamplers.NearestNeighbor)
        };
        private KeyValuePair<string, IResampler> _selectedResampler;

        public MainViewModel()
        {
            RegisterCommunicationChannel();
            LogFirstUseMetrics();
            _selectedResampler = _resamplers[1];

            SetupOutputFolder();
        }

        private async Task SetupOutputFolder()
        {

            IStorageItem item = null;

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;

            foreach (Windows.Storage.AccessCache.AccessListEntry entry in mru.Entries)
            {
                if (entry.Metadata.Equals(mruOutputFolderMetadata))
                {
                    var mruToken = entry.Token;
                    item = await mru.GetItemAsync(mruToken);
                    break;
                }
            }

            if (item == null)
            {
                OutputFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Asset Generator", CreationCollisionOption.OpenIfExists);
            }
            else
            {
                try
                {
                    OutputFolder = item as StorageFolder;
                }
                catch (Exception ex)
                {
                    OutputFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Asset Generator", CreationCollisionOption.OpenIfExists);
                }
            }

        }

        #region public properties


        public ObservableCollection<KeyValuePair<string, IResampler>> Resamplers
        {
            get
            {
                return _resamplers;
            }
        }

        public KeyValuePair<string, IResampler> SelectedResampler
        {
            get
            {
                return _selectedResampler;
            }
            set
            {
                if (_selectedResampler.Key != value.Key)
                {
                    _selectedResampler = value;
                    NotifyPropertyChanged("SelectedResampler");

                    AssetTypes.ApplyLogo();
                }
            }
        }

        public bool SaveEnabled
        {
            get
            {
                return (OutputFolder != null && OriginalLogoFile != null);
            }
        }

        public bool ShowBackgroundColorSelector
        {
            get
            {
                return _showBackgroundColorSelector;
            }
            set
            {
                if (_showBackgroundColorSelector != value)
                {
                    _showBackgroundColorSelector = value;
                    NotifyPropertyChanged("ShowBackgroundColorSelector");
                }
            }
        }

        public bool UseTransparentBackground
        {
            get
            {
                return _useTransparentBackground;
            }
            set
            {
                if (_useTransparentBackground != value)
                {
                    _useTransparentBackground = value;
                    NotifyPropertyChanged("UseTransparentBackground");
                    NotifyPropertyChanged("UseColouredBackground");
                }
            }
        }

        public bool UseColouredBackground
        {
            get
            {
                return !_useTransparentBackground;
            }
        }

        public Color BackgroundColour
        {
            get
            {
                return _backgroundColour;
            }
            set
            {
                if (_backgroundColour != value)
                {
                    _backgroundColour = value;
                    if (_backgroundColour.A == 0)
                    {
                        _backgroundColour.A = 255;
                    }
                    NotifyPropertyChanged("BackgroundColour");
                    NotifyPropertyChanged("BackgroundColourBrush");
                }
            }
        }

        public Brush BackgroundColourBrush
        {
            get
            {
                return new SolidColorBrush(_backgroundColour);
            }
        }

        public bool PreviewWithAccentColour
        {
            get
            {
                return _previewWithAccentColour;
            }
            set
            {
                if (_previewWithAccentColour != value)
                {
                    _previewWithAccentColour = value;
                    NotifyPropertyChanged("PreviewWithAccentColour");
                }
            }
        }     

        public bool ShowFeedbackControl
        {
            get
            {
                return _showFeedbackControl;
            }
            set
            {
                if (_showFeedbackControl != value)
                {
                    _showFeedbackControl = value;
                    NotifyPropertyChanged("ShowFeedbackControl");
                }
            }
        }

        public StorageFolder OutputFolder
        {
            get
            {
                return _outputFolder; 
            }
            set
            {
                if (_outputFolder != value)
                {
                    _outputFolder = value;
                    NotifyPropertyChanged("OutputFolder");
                    NotifyPropertyChanged("SaveEnabled");
                }
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
                if (_showGettingStartedControl != value)
                {
                    _showGettingStartedControl = value;
                    NotifyPropertyChanged("ShowGettingStartedControl");
                }
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
                if (_showEditCurrentImage != value)
                {
                    _showEditCurrentImage = value;
                    NotifyPropertyChanged("ShowEditCurrentImage");
                }
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

        public AssetTypeListViewModel AssetTypes
        {
            get
            {
                if (_assetTypes == null)
                {
                    _assetTypes = new AssetTypeListViewModel();
                    _assetTypes.Load();
                }
                return _assetTypes;
            }
            set
            {
                _assetTypes = value;
                NotifyPropertyChanged("AssetTypes");
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
                await LogoAsWriteableBitmap();

                AssetTypes.ApplyLogo();

                result = true;
            }

            NotifyPropertyChanged("SaveEnabled");
            return result;
        }

        /// <summary>
        /// Sets the output folder to save generated files to.
        /// </summary>
        /// <returns>
        /// Returns a bool if successful
        /// </returns>
        public async Task<bool> SetOutputFolderAsync()
        {
            var result = false;

            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                OutputFolder = folder;

                var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
                mru.Clear();
                string mruToken = mru.Add(OutputFolder, mruOutputFolderMetadata);

                Settings.LastOutputDirectoryToken = mruToken;

                result = true;
            }

            return result;
        }

        public async Task LogoAsWriteableBitmap()
        {
            ImageProperties properties = await OriginalLogoFile.Properties.GetImagePropertiesAsync();
            originalWriteableBitmap = new WriteableBitmap((int)properties.Width, (int)properties.Height);
            originalWriteableBitmap.SetSource((await OriginalLogoFile.OpenReadAsync()).AsStream().AsRandomAccessStream());
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
