using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace katalog
{
    public class CategoryManager : INotifyPropertyChanged
    {
        private ObservableCollection<string> _categories;

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public CategoryManager()
        {
            Categories = new ObservableCollection<string>();
        }

        public CategoryManager(List<string> categories)
        {
            Categories = new ObservableCollection<string>(categories);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
