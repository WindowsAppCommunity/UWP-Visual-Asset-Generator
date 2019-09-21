using MVVM;
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

        private int _leftPadding = 0;
        private int _topPadding = 0;
        private int _rightPadding = 0;
        private int _bottomPadding = 0;

        public MainViewModel mainViewModel { get; set; }

        public AssetViewModel(AssetTemplate template)
        {
            _template = template;
            ResetPadding();
            mainViewModel = App.mainViewModel;
            _savedSuccessfully = false;
        }

        public WriteableBitmap Logo
        {
            get
            {
                if (_logo == null)
                {
                    _logo = new WriteableBitmap(ImageWidth, ImageHeight);
                    _logo.Clear();
                    ApplyLogo();
                }
                return _logo;
            }
            set
            {
                if (_logo != value)
                {
                    _logo = value;
                    NotifyPropertyChanged("Logo");
                }
            }
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

        public int LeftPadding
        {
            get
            {
                return _leftPadding;
            }
            set
            {
                if (_leftPadding != value)
                {
                    _leftPadding = value;
                    NotifyPropertyChanged("LeftPadding");
                    ApplyLogo();
                }
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

        public int RightPadding
        {
            get
            {
                return _rightPadding;
            }
            set
            {
                if (_rightPadding != value)
                {
                    _rightPadding = value;
                    NotifyPropertyChanged("RightPadding");
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

        public void EraseImage()
        {
            Logo.Clear(Colors.Transparent);
        }

        public void ApplyLogo()
        {
            EraseImage();
            if (mainViewModel.originalWriteableBitmap != null)
            {
                int newLogoInsertWidth = ImageWidth - LeftPadding - RightPadding;
                int newLogoInsertHeight = ImageHeight - TopPadding - BottomPadding;

                // Figure out the ratio
                double ratioX = (double)newLogoInsertWidth / (double)mainViewModel.originalWriteableBitmap.PixelWidth;
                double ratioY = (double)newLogoInsertHeight / (double)mainViewModel.originalWriteableBitmap.PixelHeight;

                // use whichever multiplier is smaller
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                // now we can get the new height and width
                int newHeight = Convert.ToInt32(mainViewModel.originalWriteableBitmap.PixelHeight * ratio);
                int newWidth = Convert.ToInt32(mainViewModel.originalWriteableBitmap.PixelWidth * ratio);
                
                var resizedOriginal = mainViewModel.originalWriteableBitmap.Resize(
                    newWidth,
                    newHeight, 
                    WriteableBitmapExtensions.Interpolation.Bilinear);

                Logo.Blit(
                    new Rect(
                        HalfOf(Logo.PixelWidth) - HalfOf(resizedOriginal.PixelWidth),
                        HalfOf(Logo.PixelHeight) - HalfOf(resizedOriginal.PixelHeight),
                        resizedOriginal.PixelWidth,
                        resizedOriginal.PixelHeight), 
                    resizedOriginal, 
                    new Rect(
                        new Point(0, 0), 
                        new Point(resizedOriginal.PixelWidth, 
                        resizedOriginal.PixelHeight))
                    );
                NotifyPropertyChanged("Logo");
            }
        }

        private int HalfOf(int halveMe)
        {
            double result = (double)halveMe / (double)2;
            return Convert.ToInt32(result);
        }

        public void ResetPadding()
        {
            LeftPadding = _template.PreferredLeftPadding;
            TopPadding = _template.PreferredTopPadding;
            RightPadding = _template.PreferredRightPadding;
            BottomPadding = _template.PreferredBottomPadding;
        }

        public async Task<bool> SaveAssetToFileAsync()
        {
            var result = false;

            Thinking = true;
            ThinkingText = "Saving";
            await Task.Delay(App.ThinkingTiyPauseInMs);
            try
            {
                if (mainViewModel.OutputFolder != null)
                {
                    var file = await mainViewModel.OutputFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await EncodeLogoToFile(file);
                    result = true;
                    SavedSuccessfully = true;
                    ThinkingText = "Saved";
                    await Task.Delay(App.ThinkingTiyPauseInMs);
                }
            }
            catch (Exception ex)
            {
                //
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
