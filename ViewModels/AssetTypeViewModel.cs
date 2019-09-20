using MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Visual_Asset_Generator.ViewModels
{
    public class AssetTypeViewModel : ValleyBaseViewModel
    {
        private string _title;
        private AssetListViewModel _assets;

        public static string SmallTileName = "Small Tile";
        public static string MediumTileName = "Medium Tile";
        public static string WideTileName = "Wide Tile";
        public static string LargeTileName = "Large Tile";
        public static string AppIconName = "App Icon";
        public static string SplashScreenName = "Splash Screen";
        public static string BadgeLogoName = "Badge Logo";
        public static string PackageLogoTileName = "Package Logo";
        public AssetTypeViewModel(string title)
        {
            Title = title;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public AssetListViewModel Assets
        {
            get
            {
                if (_assets == null)
                {
                    _assets = new AssetListViewModel(this);
                    _assets.Load();
                }
                return _assets;
            }
            set
            {
                _assets = value;
                NotifyPropertyChanged("Assets");

            }
        }
    }
}
