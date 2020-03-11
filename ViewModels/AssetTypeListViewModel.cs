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
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Small_Tile));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Medium_Tile));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Wide_Tile));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Large_Tile));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.App_Icon));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Splash_Screen));
            //Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTitles..BadgeLogoName));
            Items.Add(new AssetTypeViewModel(AssetTypeViewModel.AssetTypes.Package_Logo));
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
            if (await ShowDialog("Save All Assets","This will overwrite any existing files in output folder","Yes","No"))
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
                    await ShowDialog("Unknown error creating assets. Sorry about that.", "I'm sorry to say so but, sadly, it's true that bang-ups and hang-ups can happen to you.  - Dr Seuss");

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
