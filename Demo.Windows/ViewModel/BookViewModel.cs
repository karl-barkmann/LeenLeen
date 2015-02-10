using CsQuery;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Demo.Windows.ViewModel
{
    public class BookViewModel : ViewModelBase
    {
        private string author;
        private string actor;
        private string category;
        private string name;
        private DateTime lastUpdateDate;
        private ObservableCollection<BookSectionViewModel> sections = new ObservableCollection<BookSectionViewModel>();
        private readonly int id;
        private Uri bookUri;

        public BookViewModel(int id, Uri bookUri)
        {
            this.id = id;
            this.bookUri = bookUri;

            ThreadPool.QueueUserWorkItem(CrawlBookContent, null);
        }

        public int Id
        {
            get { return id; }
        }

        public Uri BookUri
        {
            get { return bookUri; }
        }

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

        #region Private methods

        private void CrawlBookContent(object state)
        {
            var http = WebRequest.Create(bookUri);
            var response = http.GetResponse();
            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
            var content = sr.ReadToEnd();
            stream.Close();

            var math = Regex.Match(content, "<a href=\"lei_(\\d+)_(\\d+).htm\" class=\"now\">*.*</a>");
            if (math.Success)
            {
                var value = math.Captures[0].Value;
                Category = CQ.CreateDocument(value).Text();
            }
        }

        #endregion
    }
}
