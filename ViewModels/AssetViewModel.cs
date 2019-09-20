using MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_Visual_Asset_Generator.Data;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class AssetViewModel : ValleyBaseViewModel
    {
        private AssetTemplate _template;
        private bool _savedSuccessfully = false;

        public MainViewModel mainViewModel { get; set; }

        public AssetViewModel(AssetTemplate template)
        {
            _template = template;
            mainViewModel = App.mainViewModel;
            _savedSuccessfully = false;
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
                return _template.ImageHeight;
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
                    await mainViewModel.OutputFolder.CreateFileAsync(FileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
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
    }
}
