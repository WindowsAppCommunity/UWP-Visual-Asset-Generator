using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Items.Add(new AssetTypeViewModel("Small Tile"));
            Items.Add(new AssetTypeViewModel("Medium Tile"));
            Items.Add(new AssetTypeViewModel("Wide Tile"));
            Items.Add(new AssetTypeViewModel("Large Tile"));
            Items.Add(new AssetTypeViewModel("App Icon"));
            Items.Add(new AssetTypeViewModel("Splash Screen"));
            Items.Add(new AssetTypeViewModel("Badge Logo"));
            Items.Add(new AssetTypeViewModel("Package Logo"));
            Current = Items[0];
        }

        public void Undo()
        {
            foreach (var element in Items)
            {
                //element.Undo();
            }
        }

        public async Task CommitChanges()
        {           
            Thinking = true;
            ThinkingText = "Starting changes";
            await Task.Delay(App.ThinkingLongPauseInMs);

            //Copy as new files
            if (App.mainViewModel.OutputFolder != null)
            {
                foreach (var element in Items)
                {
                    //ThinkingText = "Copying to " + element.NewFileName + element.File.FileType;
                    //await Task.Delay(App.ThinkingShortPauseInMs);
                    //await element.CopyTo(App.mainViewModel.OutputFolder);
                }
            }
            else
            {
                await ShowDialog("No output folder selected", "To save to a new location, an 'Output Location' needs to be set.");
            }

            Thinking = true;
            ThinkingText = "Complete";
            await Task.Delay(App.ThinkingLongPauseInMs);

            Thinking = false;
            ThinkingText = string.Empty;
        }        
    }
}
