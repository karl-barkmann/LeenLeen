using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Windows.ViewModel
{
    public class BookSectionViewModel : ViewModelBase
    {
        private int sectionIndex;
        private Uri streamRequestUri;
        private string sectionName;

        public int SectionIndex
        {
            get { return sectionIndex; }
            set
            {
                if (value != sectionIndex)
                {
                    sectionIndex = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SectionName
        {
            get { return sectionName; }
            set
            {
                if (value != sectionName)
                {
                    sectionName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Uri StreamRequstUri
        {
            get { return streamRequestUri; }
            set
            {
                if (value != streamRequestUri)
                {
                    streamRequestUri = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
