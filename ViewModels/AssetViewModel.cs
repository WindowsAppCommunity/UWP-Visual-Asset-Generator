using MVVM;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UWP_Visual_Asset_Generator.Data;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class AssetViewModel : ValleyBaseViewModel
    {
        private AssetTemplate _template;
        private bool _savedSuccessfully = false;
        private WriteableBitmap _logo;
        private BitmapImage _readOnlyLogoForDisplay;

        private int _topPadding = 0;
        private int _bottomPadding = 0;

        private bool _selectedForExport = true;

        public MainViewModel mainViewModel { get; set; }

        public AssetViewModel(AssetTemplate template)
        {
            _template = template;
            ResetPadding();
            mainViewModel = App.mainViewModel;
            _savedSuccessfully = false;
            SetupInitialLogo();
        }

        public bool SelectedForExport
        {
            get
            {
                return _selectedForExport;
            }
            set
            {
                if (_selectedForExport != value)
                {
                    _selectedForExport = value;
                    NotifyPropertyChanged("SelectedForExport");
                }
            }
        }

        public WriteableBitmap Logo
        {
            get
            {                
                return _logo;
            }
            set
            {
                if (_logo != value)
                {
                    _logo = value;
                    NotifyPropertyChanged("Logo");
                    _readOnlyLogoForDisplay = null;
                    NotifyPropertyChanged("ReadOnlyLogoForDisplay");
                }
            }
        }

        public BitmapImage ReadOnlyLogoForDisplay
        {
            get
            {
                if (_readOnlyLogoForDisplay == null &&
                    Logo != null)
                {
                    UpdateReadOnlyDisplayImage();
                }
                return _readOnlyLogoForDisplay;
            }
        }

        private async Task UpdateReadOnlyDisplayImage()
        {
            _readOnlyLogoForDisplay = new BitmapImage();
            InMemoryRandomAccessStream randstream = new InMemoryRandomAccessStream();
            await Logo.ToStream(randstream, BitmapEncoder.PngEncoderId);
            _readOnlyLogoForDisplay.SetSource(randstream);
            NotifyPropertyChanged("ReadOnlyLogoForDisplay");
        }

        public string FileName
        {
            get
            {
                return _template.FileName;
            }
        }

        public bool SavedSuccessfully
        {
            get
            {
                return _savedSuccessfully;
            }
            set
            {
                _savedSuccessfully = value;
                NotifyPropertyChanged("SavedSuccessfully");
            }
        }

        public int ImageHeight
        {
            get
            {
                return _template.ImageHeight;
            }
        }

        public int ImageWidth
        {
            get
            {
                return _template.ImageWidth;
            }
        }
        
        public int TopPadding
        {
            get
            {
                return _topPadding;
            }
            set
            {
                if (_topPadding != value)
                {
                    _topPadding = value;
                    NotifyPropertyChanged("TopPadding");
                    ApplyLogo();
                }
            }
        }
        
        public int BottomPadding
        {
            get
            {
                return _bottomPadding;
            }
            set
            {
                if (_bottomPadding != value)
                {
                    _bottomPadding = value;
                    NotifyPropertyChanged("BottomPadding");
                    ApplyLogo();
                }
            }
        }

        private void SetupInitialLogo()
        {
                _logo = new WriteableBitmap(ImageWidth, ImageHeight);
                EraseImage();
           
        }

        private void EraseImage()
        {
            Logo.Clear();
            SavedSuccessfully = false;
        }

        public async Task<bool> ApplyLogo()
        {
            var result = false;

            SetupInitialLogo();
            if (mainViewModel != null &&
                mainViewModel.originalWriteableBitmap != null &&
                SelectedForExport)
            {
                try
                {
                    int newLogoInsertHeight = ImageHeight - TopPadding - BottomPadding;

                    var config = new Configuration();                    

                    var backgroundPixel = new Rgba32();
                    
                    if (mainViewModel.UseTransparentBackground)
                    {
                        backgroundPixel = SixLabors.ImageSharp.Color.Transparent;
                    }
                    else
                    {
                        backgroundPixel.R = mainViewModel.BackgroundColour.R;
                        backgroundPixel.G = mainViewModel.BackgroundColour.G;
                        backgroundPixel.B = mainViewModel.BackgroundColour.B;
                        backgroundPixel.A = mainViewModel.BackgroundColour.A;                    
                    }

                    var newLogo = new Image<Rgba32>(config, ImageWidth, ImageHeight, backgroundPixel);

                    var options = new ResizeOptions()
                    {
                        Mode = ResizeMode.Max,
                        Position = AnchorPositionMode.Center,
                        Size = new SixLabors.Primitives.Size(ImageWidth - TwentyPercentOf(ImageWidth), newLogoInsertHeight),
                        Sampler = mainViewModel.SelectedResampler.Value
                    };                    

                    var inStream = await mainViewModel.OriginalLogoFile.OpenReadAsync();
                    var resizedOriginal = Image<Rgba32>.Load(inStream.AsStreamForRead());

                    //resize the image to fit the GIF frame bounds
                    resizedOriginal.Mutate(r => r.Resize(options));

                    //Get the top left point within the logo bounds
                    var left = HalfOf(ImageWidth) - HalfOf(resizedOriginal.Width);                    
                    var top = HalfOf(ImageHeight) - HalfOf(resizedOriginal.Height);

                    newLogo.Mutate(w => w.DrawImage(resizedOriginal, new SixLabors.Primitives.Point(left, top), 1));
                    InMemoryRandomAccessStream myStream = new InMemoryRandomAccessStream();
                    
                    SixLabors.ImageSharp.Formats.Png.PngEncoder encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                    newLogo.SaveAsPng(myStream.AsStream(), encoder);
                    myStream.Seek(0);
                    Logo = await BitmapFactory.FromStream(myStream);

                    NotifyPropertyChanged("Logo");
                    result = true;
                }
                catch (Exception ex)
                {

                }
                SavedSuccessfully = false;
            }

            return result;
        }

        public int HalfOf(int halveMe)
        {
            double result = (double)halveMe / (double)2;
            return Convert.ToInt32(result);
        }

        private int TwentyPercentOf(int twentyPercentOfMe)
        {
            double result = (double)twentyPercentOfMe / (double)5;
            return Convert.ToInt32(result);
        }

        public void ResetPadding()
        {
            if (_template.PaddingIsRecommended)
            {
                _topPadding = HalfOf(HalfOf(_template.ImageHeight));
                _bottomPadding = _topPadding;
            }
            else
            {
                _topPadding = 0;
                _bottomPadding = 0;
            }
            NotifyPropertyChanged("TopPadding");
            NotifyPropertyChanged("BottompPadding");
            ApplyLogo();
        }

        public void ZeroPadding()
        {
            _topPadding = 0;
            _bottomPadding = 0;
            NotifyPropertyChanged("TopPadding");
            NotifyPropertyChanged("BottompPadding");
            ApplyLogo();
        }

        public async Task<bool> SaveAssetToFileAsync()
        {
            var result = false;

            if (SelectedForExport)
            {
                Thinking = true;
                ThinkingText = "Saving";
                await Task.Delay(App.ThinkingTinyPauseInMs);

                try
                {
                    if (mainViewModel.OutputFolder != null)
                    {
                        var file = await mainViewModel.OutputFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                        await EncodeLogoToFile(file);
                        result = true;
                        SavedSuccessfully = true;
                        ThinkingText = "Saved";
                        await Task.Delay(App.ThinkingTinyPauseInMs);
                    }
                }
                catch (Exception ex)
                {
                    //
                }
            }

            Thinking = false;
            ThinkingText = string.Empty;
            return result;
        }

        private async Task EncodeLogoToFile(StorageFile outFile)
        {

            using (IRandomAccessStream stream = await outFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                Stream pixelStream = Logo.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Logo.PixelWidth, (uint)Logo.PixelHeight,
                    96.0,
                    96.0,
                    pixels);
                await encoder.FlushAsync();

            }
        }
    }
}
