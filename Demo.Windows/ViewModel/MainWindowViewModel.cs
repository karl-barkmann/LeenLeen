using Leen.Practices.Mvvm;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : ViewModelBase
    {
        private int _state;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            State = 1;
            ShowTingCrawlerCommand = new RelayCommand(OnShowTingCrawler);
            ShowNgaCrawlerCommand = new RelayCommand(OnShowNgaCrawler);
            SearchCommand = new RelayCommand<string>(OnSearch,OnCanSerach);
        }

        public int State
        {
            get { return _state; }
            set
            {
                SetProperty(ref _state, value, () => State);
            }
        }

        public ICommand SearchCommand { get; }

        public ICommand ShowTingCrawlerCommand { get; set; }

        public ICommand ShowNgaCrawlerCommand { get; set; }

        private void OnShowTingCrawler()
        {
            var crawler = new TingCrawlerWindowViewModel();
            UIService.ShowDialog(crawler, this);
        }

        private void OnShowNgaCrawler()
        {
            var crawler = new NgaCrawlerWindowViewModel();
            UIService.ShowDialog(crawler, this);
        }

        private bool OnCanSerach(string keywords)
        {
            return true;
        }

        private void OnSearch(string keywords)
        {
            UIService.ShowInfoMessage($"ËÑË÷¹Ø¼ü×Ö£º{keywords}", "ÌáÊ¾", this);
        }
    }
}