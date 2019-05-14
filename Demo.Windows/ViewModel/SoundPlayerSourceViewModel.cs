using Leen.Practices.Mvvm;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Demo.Windows
{
    public class SoundPlayerSourceViewModel : ViewModelBase
    {
        public SoundPlayerSourceViewModel(string request)
        {
            RequestUrl = request;
            PlayCommand = new RelayCommand(Play);
        }

        private void Play()
        {
            try
            {
                var http = WebRequest.Create(RequestUrl);
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream, Encoding.UTF8);
                var content = sr.ReadToEnd();
                stream.Dispose();
                var keyIndex = "url[3]=";
                var realUrlIndex = content.IndexOf(keyIndex);
                var realUrlIndex1 = content.IndexOf(";", realUrlIndex);

                var realUrl = content.Substring(realUrlIndex + keyIndex.Length + 2, realUrlIndex1 - realUrlIndex - keyIndex.Length - 3);
                string decodeUrl = System.Web.HttpUtility.UrlDecode(realUrl);
                SoundStreamUri = String.Format("http://t33.tingchina.com{0}", decodeUrl);

                //Process.Start("iexplore.exe", SoundStreamUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show("播放失败" + ex.Message);
            }
        }

        private string requestUrl = "";
        private string soundStreamUri = "";

        public string RequestUrl
        {
            get { return requestUrl; }
            set
            {
                if (value != requestUrl)
                {
                    requestUrl = value;
                    RaisePropertyChanged(() => RequestUrl);
                }
            }
        }

        public string SoundStreamUri
        {
            get { return soundStreamUri; }
            set
            {
                if (value != soundStreamUri)
                {
                    soundStreamUri = value;
                    RaisePropertyChanged(() => SoundStreamUri);
                }
            }
        }

        public ICommand PlayCommand { get; set; }
    }
}
