using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Windows.ViewModel
{
    class BookViewModel : ViewModelBase
    {
        private string author;
        private string actor;
        private string category;
        private string name;
        private DateTime lastUpdateDate;
        private ObservableCollection<BookSectionViewModel> sections = new ObservableCollection<BookSectionViewModel>();

        public string Author
        {
            get { return author; }
            set
            {
                if (value != author)
                {
                    author = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Actor
        {
            get { return actor; }
            set
            {
                if (value != actor)
                {
                    actor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Category
        {
            get { return category; }
            set
            {
                if (value != category)
                {
                    category = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DateTime LastUpdateDate
        {
            get { return lastUpdateDate; }
            set
            {
                if (value != lastUpdateDate)
                {
                    lastUpdateDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<BookSectionViewModel> Sections
        {
            get { return sections; }
            set
            {
                if (value != sections)
                {
                    sections = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
