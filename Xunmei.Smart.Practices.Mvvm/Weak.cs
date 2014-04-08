using System.ComponentModel;
using Xunmei.Smart.Common;

namespace Xunmei.AlarmCentre.Client.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyChangedEventListener :
        WeakEventListener<INotifyPropertyChanged, PropertyChangedEventArgs>
    {
        public PropertyChangedEventListener(
            INotifyPropertyChanged source,
            PropertyChangedEventHandler handler)
            : base(source, handler)
        {
            source.PropertyChanged += OnEvent;
        }

        protected override void StopListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged -= OnEvent;
        }
    }
}
