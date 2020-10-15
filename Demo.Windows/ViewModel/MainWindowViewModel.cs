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
        private string _tile;
        private MainWindowViewModel _inner;
        private MainWindowViewModel _nest;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            ShowTingCrawlerCommand = new RelayCommand(OnShowTingCrawler);
            ShowNgaCrawlerCommand = new RelayCommand(OnShowNgaCrawler);
            ShowSimpleTraderCrawlerCommand = new RelayCommand(OnShowSimpleTraderCrawler);
            SearchCommand = new RelayCommand<string>(OnSearch, OnCanSerach);
            Title = "Shell";
        }

        public string Title
        {
            get { return _tile; }
            set
            {
                SetProperty(ref _tile, value, () => Title);
            }
        }

        public MainWindowViewModel Inner
        {
            get { return _inner; }
            set
            {
                SetProperty(ref _inner, value, () => Inner);
            }
        }

        public MainWindowViewModel Nest
        {
            get { return _nest; }
            set
            {
                SetProperty(ref _nest, value, () => Nest);
            }
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
        [WatchOn(nameof(IsBusy))]
        public ICommand SearchCommand { get; }

        public ICommand ShowTingCrawlerCommand { get; set; }

        public ICommand ShowNgaCrawlerCommand { get; set; }

        public ICommand ShowSimpleTraderCrawlerCommand { get; set; }

        private void OnShowTingCrawler()
        {
            Inner = new MainWindowViewModel();
            Inner.Nest = new MainWindowViewModel();
            Watch(() => Inner.Nest.State, (oldVal, newVal) =>
            {
                Console.WriteLine($"ÊôÐÔ¼àÌý£º{nameof(Inner.Nest.State)}=> {newVal}");
            }, true);
            Inner.Nest.State++;
            State++;
            //var crawler = new TingCrawlerWindowViewModel();
            //UIService.ShowDialog(crawler, this);
        }

        private void OnShowNgaCrawler()
        {
            IsBusy = true;
            var crawler = new MainWindowViewModel();
            crawler.Title = "Show at " + DateTime.Now.ToLongTimeString();
            UIService.Show(crawler, this);
        }

        private void OnShowSimpleTraderCrawler()
        {
            IsBusy = false;
            var crawler = new MainWindowViewModel();
            crawler.Title = "ShowDialog at " + DateTime.Now.ToLongTimeString();
            UIService.ShowDialog(crawler, this);
        }

        private bool OnCanSerach(string keywords)
        {
            return State < 3 && !IsBusy;
        }

        [WatchOn(nameof(State))]
        [WatchOn(nameof(IsBusy))]
        private void OnStateChange(object oldVal, object newVal)
        {
            Console.WriteLine("OnStateChange");
        }

        private void OnSearch(string keywords)
        {
            State++;
            UIService.ShowInfoMessage($"ËÑË÷¹Ø¼ü×Ö£º{keywords}", "ÌáÊ¾", this);
        }
    }
}