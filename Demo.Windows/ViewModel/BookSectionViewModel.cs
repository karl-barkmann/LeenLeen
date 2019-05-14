using Leen.Practices.Mvvm;
using System;

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
                    RaisePropertyChanged(nameof(SectionIndex));
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
                    RaisePropertyChanged(nameof(SectionName));
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
                    RaisePropertyChanged(nameof(StreamRequstUri));
                }
            }
        }
    }
}
