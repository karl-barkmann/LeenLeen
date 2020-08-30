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
            ShowTingCrawlerCommand = new RelayCommand(OnShowTingCrawler);
            ShowNgaCrawlerCommand = new RelayCommand(OnShowNgaCrawler);
            ShowSimpleTraderCrawlerCommand = new RelayCommand(OnShowSimpleTraderCrawler);
            SearchCommand = new RelayCommand<string>(OnSearch, OnCanSerach);
        }

        public int State
        {
            get { return _state; }
            set
            {
                SetProperty(ref _state, value, () => State);
            }
        }

        [WatchOn(nameof(State))]
        public ICommand SearchCommand { get; }

        public ICommand ShowTingCrawlerCommand { get; set; }

        public ICommand ShowNgaCrawlerCommand { get; set; }

        public ICommand ShowSimpleTraderCrawlerCommand { get; set; }

        private void OnShowTingCrawler()
        {
            Watch(() => State, (oldVal, newVal) =>
            {
                Console.WriteLine($"ÊôÐÔ¼àÌý£º{nameof(State)}=> {newVal}");
            }, true);
            State++;
            //var crawler = new TingCrawlerWindowViewModel();
            //UIService.ShowDialog(crawler, this);
        }

        private void OnShowNgaCrawler()
        {
            var crawler = new NgaCrawlerWindowViewModel();
            UIService.ShowDialog(crawler, this);
        }

        private void OnShowSimpleTraderCrawler()
        {
            var crawler = new SimpleTraderCrawlerWindowViewModel();
            UIService.ShowDialog(crawler, this);
        }

        private bool OnCanSerach(string keywords)
        {
            return State < 3;
        }

        [WatchOn(nameof(State), typeof(int))]
        private void OnStateChange()
        {

        }

        private void OnSearch(string keywords)
        {
            State++;
            UIService.ShowInfoMessage($"ËÑË÷¹Ø¼ü×Ö£º{keywords}", "ÌáÊ¾", this);
        }
    }
}