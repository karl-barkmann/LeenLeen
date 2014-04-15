using System.ComponentModel;
using Smart.Common;

namespace Smart.Practices.Mvvm
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
