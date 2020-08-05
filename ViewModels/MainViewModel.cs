using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Helpers;
using MVVM;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
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
        private bool _isFirstRun = SystemInformation.IsFirstRun;
        private Color _backgroundColour = Color.FromArgb(0, 0, 0, 0);

        public const string mruOutputFolderMetadata = "OutputFolder";

        private StorageFile _originalLogoFile;
        private ImageSource _originalLogoImageSource;
        private StorageFolder _outputFolder;
        public WriteableBitmap originalWriteableBitmap;

        private AssetTypeListViewModel _assetTypes;

        private const string WinStoreProductID = "9MZ6QRQTDKF2";


        private ObservableCollection<KeyValuePair<string, IResampler>> _resamplers = new ObservableCollection<KeyValuePair<string, IResampler>>()
        { 
            new KeyValuePair<string, IResampler>("Average",KnownResamplers.Box),
            new KeyValuePair<string, IResampler>("Bicubic", KnownResamplers.Bicubic ),
            new KeyValuePair<string, IResampler>("Nearest Neighbour", KnownResamplers.NearestNeighbor),
            new KeyValuePair<string, IResampler>("Box", KnownResamplers.Box),
            new KeyValuePair<string, IResampler>("CatmullRom", KnownResamplers.CatmullRom ),
            new KeyValuePair<string, IResampler>("Hermite", KnownResamplers.Hermite ),
            new KeyValuePair<string, IResampler>("Lanczos2", KnownResamplers.Lanczos2 ),
            new KeyValuePair<string, IResampler>("Lanczos3", KnownResamplers.Lanczos3),
            new KeyValuePair<string, IResampler>("Lanczos5", KnownResamplers.Lanczos5 ),
            new KeyValuePair<string, IResampler>("Lanczos8", KnownResamplers.Lanczos8 ),
            new KeyValuePair<string, IResampler>("MitchellNetravali", KnownResamplers.MitchellNetravali ),
            new KeyValuePair<string, IResampler>("Robidoux", KnownResamplers.Robidoux ),
            new KeyValuePair<string, IResampler>("RobidouxSharp", KnownResamplers.RobidouxSharp ),
            new KeyValuePair<string, IResampler>("Spline", KnownResamplers.Spline ),
            new KeyValuePair<string, IResampler>("Triangle" , KnownResamplers.Triangle ),
            new KeyValuePair<string, IResampler>("Welch", KnownResamplers.Welch )
        };
        private KeyValuePair<string, IResampler> _selectedResampler;

        private ObservableCollection<KeyValuePair<string, PixelColorBlendingMode>> _blendingModes = new ObservableCollection<KeyValuePair<string, PixelColorBlendingMode>>()
        { 
            new KeyValuePair<string, PixelColorBlendingMode>("Add",PixelColorBlendingMode.Add),
            new KeyValuePair<string, PixelColorBlendingMode>("Darken",PixelColorBlendingMode.Darken),
            new KeyValuePair<string, PixelColorBlendingMode>("HardLight",PixelColorBlendingMode.HardLight),
            new KeyValuePair<string, PixelColorBlendingMode>("Lighten",PixelColorBlendingMode.Lighten),
            new KeyValuePair<string, PixelColorBlendingMode>("Multiply",PixelColorBlendingMode.Multiply),
            new KeyValuePair<string, PixelColorBlendingMode>("Normal",PixelColorBlendingMode.Normal),
            new KeyValuePair<string, PixelColorBlendingMode>("Overlay",PixelColorBlendingMode.Overlay),
            new KeyValuePair<string, PixelColorBlendingMode>("Screen",PixelColorBlendingMode.Screen),
            new KeyValuePair<string, PixelColorBlendingMode>("Subtract",PixelColorBlendingMode.Subtract)
        };
        private KeyValuePair<string, PixelColorBlendingMode> _selectedBlendingMode;


        private ObservableCollection<KeyValuePair<string, PixelAlphaCompositionMode>> _alphaModes = new ObservableCollection<KeyValuePair<string, PixelAlphaCompositionMode>>()
        { 
            new KeyValuePair<string, PixelAlphaCompositionMode>("The Logo where they don't overlap otherwise dest in overlapping parts.",PixelAlphaCompositionMode.DestAtop),
            new KeyValuePair<string, PixelAlphaCompositionMode>("The Background over the Logo.",PixelAlphaCompositionMode.DestOver),
            new KeyValuePair<string, PixelAlphaCompositionMode>("Returns the Logo colors.",PixelAlphaCompositionMode.Src),
            new KeyValuePair<string, PixelAlphaCompositionMode>("The Background where the Background and Logo overlap.",PixelAlphaCompositionMode.SrcOut),
            new KeyValuePair<string, PixelAlphaCompositionMode>("Returns the Background over the Logo.",PixelAlphaCompositionMode.SrcOver),
            new KeyValuePair<string, PixelAlphaCompositionMode>("Clear where they overlap.",PixelAlphaCompositionMode.Xor)
        };
        private KeyValuePair<string, PixelAlphaCompositionMode> _selectedAlphaMode;

        private ObservableCollection<KeyValuePair<string, PngCompressionLevel>> _pngCompressionOptions = new ObservableCollection<KeyValuePair<string, PngCompressionLevel>>()
        { 
            new KeyValuePair<string, PngCompressionLevel>("Fastest (level 1)", PngCompressionLevel.BestSpeed),
            new KeyValuePair<string, PngCompressionLevel>("Best (level 9)", PngCompressionLevel.BestCompression ),
            new KeyValuePair<string, PngCompressionLevel>("Default (Level 6)", PngCompressionLevel.DefaultCompression),
            new KeyValuePair<string, PngCompressionLevel>("No Compression (level 0)", PngCompressionLevel.NoCompression)
        };
        private KeyValuePair<string, PngCompressionLevel> _selectedPNGCompression;

        public MainViewModel()
        {
            LogFirstUseMetrics();
            _selectedResampler = _resamplers[1];
            _selectedBlendingMode = _blendingModes[5];
            _selectedAlphaMode = _alphaModes[3];
            _selectedPNGCompression = _pngCompressionOptions[2];

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


        public ObservableCollection<KeyValuePair<string, PngCompressionLevel>> PNGCompressionOptions
        {
            get
            {
                return _pngCompressionOptions;
            }
        }

        public KeyValuePair<string, PngCompressionLevel> SelectedPNGCompression
        {
            get
            {
                return _selectedPNGCompression;
            }
            set
            {
                if (_selectedPNGCompression.Key != value.Key)
                {
                    _selectedPNGCompression = value;
                    NotifyPropertyChanged("SelectedPNGCompression");
                }
            }
        }

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

        public ObservableCollection<KeyValuePair<string, PixelColorBlendingMode>> BlendingModes
        {
            get
            {
                return _blendingModes;
            }
        }

        public KeyValuePair<string, PixelColorBlendingMode> SelectedBlendingMode
        {
            get
            {
                return _selectedBlendingMode;
            }
            set
            {
                if (_selectedBlendingMode.Key != value.Key)
                {
                    _selectedBlendingMode = value;
                    NotifyPropertyChanged("SelectedBlendingMode");

                    AssetTypes.ApplyLogo();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, PixelAlphaCompositionMode>> AlphaModes
        {
            get
            {
                return _alphaModes;
            }
        }

        public KeyValuePair<string, PixelAlphaCompositionMode> SelectedAlphaMode
        {
            get
            {
                return _selectedAlphaMode;
            }
            set
            {
                if (_selectedAlphaMode.Key != value.Key)
                {
                    _selectedAlphaMode = value;
                    NotifyPropertyChanged("SelectedAlphaMode");

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

        public bool IsFirstRun
        {
            get
            {
                return _isFirstRun;
            }
            set
            {
                if (_isFirstRun != value)
                {
                    _isFirstRun = value;
                    NotifyPropertyChanged("IsFirstRun");
                }
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
                    NotifyPropertyChanged("PreviewWithAccentColourBrush");
                }
            }
        }    
        
        public Color PreviewWithAccentColourBrush
        {
            get
            {
                if (PreviewWithAccentColour)
                {
                    try
                    {
                        var color = (Color)Application.Current.Resources["SystemAccentColor"];
                        return color;
                    }
                    catch (Exception ex)
                    {
                        return Colors.RoyalBlue;
                    }
                }
                else
                {
                    return Colors.Transparent;
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
        #endregion functions

    }

}
