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

        public MainViewModel mainViewModel { get; set; }

        public AssetViewModel(AssetTemplate template)
        {
            _template = template;
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
                    _logo.Clear(Colors.Red);
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

        public void EraseImage()
        {
            Logo.Clear(Colors.Blue);
        }

        public void ApplyLogo()
        {
            EraseImage();
            if (mainViewModel.originalWriteableBitmap != null)
            {
                var resizedOriginal = mainViewModel.originalWriteableBitmap.Clone();
                resizedOriginal.Resize(ImageWidth - 24, ImageHeight - 24, WriteableBitmapExtensions.Interpolation.Bilinear);

                Logo.Blit(new Rect(new Point(0, 0), new Point(resizedOriginal.PixelWidth, resizedOriginal.PixelHeight)), resizedOriginal, new Rect(new Point(0, 0), new Point(resizedOriginal.PixelWidth, resizedOriginal.PixelHeight)));
                NotifyPropertyChanged("Logo");
            }
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
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)Logo.PixelWidth, (uint)Logo.PixelHeight,
                    96.0,
                    96.0,
                    pixels);
                await encoder.FlushAsync();

            }
        }
    }
}
