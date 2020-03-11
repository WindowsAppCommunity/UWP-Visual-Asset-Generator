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
        private string _notes;
        private AssetListViewModel _assets;
        public AssetTypes AssetType;

        public enum AssetTypes
        {Small_Tile, Medium_Tile, Wide_Tile, Large_Tile, App_Icon, Splash_Screen, Badge_Logo, Package_Logo }

        //public static string SmallTileName = "Small Tile";
        //public static string MediumTileName = "Medium Tile";
        //public static string WideTileName = "Wide Tile";
        //public static string LargeTileName = "Large Tile";
        //public static string AppIconName = "App Icon";
        //public static string SplashScreenName = "Splash Screen";
        //public static string BadgeLogoName = "Badge Logo";
        //public static string PackageLogoTileName = "Package Logo";

        public AssetTypeViewModel(AssetTypes assetType)
        {
            AssetType = assetType;

            switch (AssetType)
            {
                case AssetTypes.Small_Tile:
                    _title = "Small Tile";
                    _notes = "Used for Start Menu";
                    break;
                case AssetTypes.Medium_Tile:
                    _title = "Medium Tile";
                    _notes = "Used for Start Menu and Store Listing (unless uverridden in store submission)";
                    break;
                case AssetTypes.Wide_Tile:
                    _title = "Wide Tile";
                    _notes = "Used for Start Menu";
                    break;
                case AssetTypes.Large_Tile:
                    _title = "Large Tile";
                    _notes = "Used for Start Menu and Store Listing (unless uverridden in store submission)";
                    break;
                case AssetTypes.App_Icon:
                    _title = "App Icon";
                    _notes = "App list in start menu, task bar, task manager";
                    break;
                case AssetTypes.Splash_Screen:
                    _title = "Splash Screen";
                    _notes = "The app's splash screen";
                    break;
                case AssetTypes.Badge_Logo:
                    _title = "Badge Logo";
                    _notes = "Your app's tiles";
                    break;
                case AssetTypes.Package_Logo:
                    _title = "Package Logo";
                    _notes = "App installer, Partner Center, the 'Report an app' option in the Store, the 'Write a review' option in the Store";
                    break;
                default: throw new Exception("Pleass supply a valid asset title"); 
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Notes
        {
            get
            {
                return _notes;
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
