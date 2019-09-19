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
