using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class AssetTypeListViewModel : ValleyBaseViewModel
    {
        private ObservableCollection<AssetTypeViewModel> _items;
        private AssetTypeViewModel _current;

        public AssetTypeListViewModel()
        {
            _items = new ObservableCollection<AssetTypeViewModel>();
        }

        public ObservableCollection<AssetTypeViewModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    NotifyPropertyChanged("Items");
                }
            }
        }

        public AssetTypeViewModel Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current != value)
                {
                    _current = value;
                    NotifyPropertyChanged("Current");
                }
            }
        }

        public void Load()
        {
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.SmallTileName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.MediumTileName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.WideTileName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.LargeTileName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AppIconName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.SplashScreenName));
            //Items.Add(new AssetTypeViewModel(AssetTypeViewModel.BadgeLogoName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.PackageLogoTileName));
            Current = Items[0];
        }

        public async Task ApplyLogo()
        {
            if (App.mainViewModel.originalWriteableBitmap != null)
            {
                foreach (var element in Items)
                {
                    element.Assets.ApplyLogo();
                }
            }
        }

        public async Task UpdateAllPaddingAsync(bool useReccomendedPadding = false)
        {
            foreach (var assetType in Items)
            {
                foreach (var asset in assetType.Assets.Items)
                {
                    if (useReccomendedPadding)
                    {
                        asset.ResetPadding();
                    }
                    else
                    {
                        asset.ZeroPadding();
                    }
                }
            }
        }

        public async Task SaveAllAsync()
        {
            if (await ShowDialog("Save All Assets","This will overwrite any existing files in ourput folder","Yes","No"))
            {
                try
                {
                    App.mainViewModel.Thinking = true;
                    App.mainViewModel.ThinkingText = "Saving Assets";
                    await Task.Delay(App.ThinkingMediumPauseInMs);

                    //Copy as new files
                    if (App.mainViewModel.OutputFolder != null)
                    {
                        foreach (var element in Items)
                        {
                            await element.Assets.SaveAllAssetsToFileAsync();
                        }

                        App.mainViewModel.Thinking = true;
                        App.mainViewModel.ThinkingText = "Complete";
                        await Task.Delay(App.ThinkingMediumPauseInMs);
                        
                        await Windows.System.Launcher.LaunchFolderAsync(App.mainViewModel.OutputFolder);
                    }
                    else
                    {
                        await ShowDialog("No output folder selected", "To save to a new location, an 'Output Location' needs to be set.");

                        App.mainViewModel.Thinking = true;
                        App.mainViewModel.ThinkingText = "Error";
                        await Task.Delay(App.ThinkingMediumPauseInMs);
                    }
                }
                catch (Exception eex)
                {
                    await ShowDialog("Error creating assets", "An unknown error has occurred.  Sorry.");

                    App.mainViewModel.Thinking = true;
                    App.mainViewModel.ThinkingText = "Error";
                    await Task.Delay(App.ThinkingMediumPauseInMs);
                }
                finally
                {
                    App.mainViewModel.Thinking = false;
                    App.mainViewModel.ThinkingText = string.Empty;
                }
            }
        }        
    }
}
