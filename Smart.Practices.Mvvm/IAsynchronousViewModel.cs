
namespace Smart.Practices.Mvvm
{
    public interface IAsynchronousViewModel : IViewModel
    {
        bool IsBusy { get; }

        string BusyMessage { get; }
    }
}
