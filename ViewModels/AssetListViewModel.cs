using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_Visual_Asset_Generator.Data;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class AssetListViewModel : ValleyBaseViewModel
    {
        AssetTypeViewModel _parent;
        private ObservableCollection<AssetViewModel> _items;
        private AssetViewModel _current;

        public AssetListViewModel(AssetTypeViewModel parent)
        {
            _parent = parent;
        }

        public ObservableCollection<AssetViewModel> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<AssetViewModel>();
                }
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

        public AssetViewModel Current
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
            if (_parent.Title.Equals(AssetTypeViewModel.SmallTileName))
            {
                Items.Add(new AssetViewModel(new SmallTileScale100()));
                Items.Add(new AssetViewModel(new SmallTileScale125()));
                Items.Add(new AssetViewModel(new SmallTileScale150()));
                Items.Add(new AssetViewModel(new SmallTileScale200()));
                Items.Add(new AssetViewModel(new SmallTileScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.MediumTileName))
            {
                Items.Add(new AssetViewModel(new MediumTileScale100()));
                Items.Add(new AssetViewModel(new MediumTileScale125()));
                Items.Add(new AssetViewModel(new MediumTileScale150()));
                Items.Add(new AssetViewModel(new MediumTileScale200()));
                Items.Add(new AssetViewModel(new MediumTileScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.WideTileName))
            {
                Items.Add(new AssetViewModel(new WideTileScale100()));
                Items.Add(new AssetViewModel(new WideTileScale125()));
                Items.Add(new AssetViewModel(new WideTileScale150()));
                Items.Add(new AssetViewModel(new WideTileScale200()));
                Items.Add(new AssetViewModel(new WideTileScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.LargeTileName))
            {
                Items.Add(new AssetViewModel(new LargeTileScale100()));
                Items.Add(new AssetViewModel(new LargeTileScale125()));
                Items.Add(new AssetViewModel(new LargeTileScale150()));
                Items.Add(new AssetViewModel(new LargeTileScale200()));
                Items.Add(new AssetViewModel(new LargeTileScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.AppIconName))
            {
                Items.Add(new AssetViewModel(new AppIconScale100()));
                Items.Add(new AssetViewModel(new AppIconScale125()));
                Items.Add(new AssetViewModel(new AppIconScale150()));
                Items.Add(new AssetViewModel(new AppIconScale200()));
                Items.Add(new AssetViewModel(new AppIconScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.SplashScreenName))
            {
                Items.Add(new AssetViewModel(new SplashScreenScale100()));
                Items.Add(new AssetViewModel(new SplashScreenScale125()));
                Items.Add(new AssetViewModel(new SplashScreenScale150()));
                Items.Add(new AssetViewModel(new SplashScreenScale200()));
                Items.Add(new AssetViewModel(new SplashScreenScale400()));
            }
            if (_parent.Title.Equals(AssetTypeViewModel.BadgeLogoName))
            {
                //Todo...
            }
            if (_parent.Title.Equals(AssetTypeViewModel.PackageLogoTileName))
            {
                Items.Add(new AssetViewModel(new PackageLogoScale100()));
                Items.Add(new AssetViewModel(new PackageLogoScale125()));
                Items.Add(new AssetViewModel(new PackageLogoScale150()));
                Items.Add(new AssetViewModel(new PackageLogoScale200()));
                Items.Add(new AssetViewModel(new PackageLogoScale400()));
            }
        }

        public void ApplyLogo()
        {
            foreach (var element in Items)
            {
                element.ApplyLogo();
            }
        }

        public async Task SaveAllAssetsToFileAsync()
        {
            foreach (var element in Items)
            {
                await element.SaveAssetToFileAsync();
            }
        }
    }
}
