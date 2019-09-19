using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Items.Add(new AssetViewModel("a"));
            Items.Add(new AssetViewModel("b"));
            Items.Add(new AssetViewModel("c"));
            Items.Add(new AssetViewModel("d"));
            Items.Add(new AssetViewModel("e"));
        }
    }
}
