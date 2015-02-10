using System.ComponentModel;

namespace Smart.Practices.Mvvm
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void CleanUp();
    }
}
